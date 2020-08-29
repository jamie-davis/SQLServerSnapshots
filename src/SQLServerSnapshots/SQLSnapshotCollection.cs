using System;
using System.Collections.Generic;
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
        private Dictionary<string, SchemaStructure> _schemas = new Dictionary<string, SchemaStructure>();

        public SqlSnapshotCollection()
        {
            _collection = new SnapshotCollection();
        }

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
            foreach (var schema in _schemas.Values)
            {
                SnapshotTableDefiner.Define(_collection, schema);
            }
        }

        public void ReportChanges(string before, string after, Output output)
        {
            _collection.ReportChanges(before, after, output);
        }

        public void GetSnapshotReport(string name, Output output, params string[] tables)
        {
            _collection.GetSnapshotReport(name, output, tables);
        }

        public TableDefiner DefineTable(string tableName)
        {
            return _collection.DefineTable(tableName);
        }

        public void GetSchemaReport(Output output)
        {
            ConfigureCollection();
            _collection.GetSchemaReport(output);
        }

    }
}
