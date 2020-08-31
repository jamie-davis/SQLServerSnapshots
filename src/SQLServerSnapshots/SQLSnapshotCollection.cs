using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using SnapshotTests;
using SnapshotTests.Snapshots;
using SQLServerSnapshots.Schemas;
using SQLServerSnapshots.Snapshots;
using TestConsoleLib;

namespace SQLServerSnapshots
{
    public class SqlSnapshotCollection
    {
        private SnapshotCollection _collection;
        private readonly Dictionary<string, SchemaStructure> _schemas = new Dictionary<string, SchemaStructure>();
        private readonly List<DefinitionSet> _overrides = new List<DefinitionSet>();

        public void ConfigureSchema(string server, string database, string schema)
        {
            _schemas[schema] = SchemaStructureLoader.Load(server, database, schema);
        }

        public SnapshotBuilder Snapshot(string connectionString, string snapshotName)
        {
            ConfigureCollection();
            var builder = _collection.NewSnapshot(snapshotName);
            DbSnapshotMaker.Make(connectionString, builder, _schemas.Values);
            return builder;
        }

        private void ConfigureCollection()
        {
            if (_collection != null) return;

            _collection = new SnapshotCollection();

            ApplyConfig(_collection);
        }

        private void ApplyConfig(SnapshotCollection snapshotCollection)
        {
            foreach (var schema in _schemas.Values)
            {
                SnapshotTableDefiner.Define(snapshotCollection, schema);
            }

            foreach (var definitionSet in _overrides)
            {
                snapshotCollection.ApplyDefinitions(definitionSet);
            }
        }

        public void ReportChanges(string before, string after, Output output)
        {
            ConfigureCollection();
            _collection.ReportChanges(before, after, output);
        }

        public void GetSnapshotReport(string name, Output output, params string[] tables)
        {
            ConfigureCollection();
            _collection.GetSnapshotReport(name, output, tables);
        }

        public TableDefiner DefineTable(string tableName)
        {
            ConfigureCollection();
            return _collection.DefineTable(tableName);
        }

        public void GetSchemaReport(Output output, bool verbose = false)
        {
            var collection = _collection;
            if (collection == null)
            {
                collection = new SnapshotCollection();
                ApplyConfig(collection);                
            }
            collection.GetSchemaReport(output, verbose);
        }

        public void LoadSchemaOverrides(Type containerType)
        {
            _overrides.Add(SnapshotDefinitionLoader.Load(containerType));
        }

        public void LoadSchemaOverrides(Assembly assembly)
        {
            _overrides.Add(SnapshotDefinitionLoader.Load(assembly));
        }
    }
}
