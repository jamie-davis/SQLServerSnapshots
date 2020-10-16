using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using SnapshotTests;
using SnapshotTests.Snapshots;
using SQLServerSnapshots.Exceptions;
using SQLServerSnapshots.Schemas;
using SQLServerSnapshots.Snapshots;
using SQLServerSnapshots.Utilities;
using TestConsoleLib;

namespace SQLServerSnapshots
{
    /// <summary>
    /// Container for SQL Server snapshots.
    /// <para/>
    /// After creating an instance of this class, it needs to be initialised with schema information using <see cref="ConfigureSchema"/>. This will
    /// analyse the tables in the database in the specified schema and configure the collection to accept snapshot data.
    /// <para/>
    /// If required, additional configuration can be loaded from configuration classes using the <see cref="LoadSchemaOverrides(Type)"/> or <see cref="LoadSchemaOverrides(Assembly)"/> overloads,
    /// or via calls to <see cref="DefineTable"/>.
    /// <para/>
    /// Once configuration is loaded, <see cref="Snapshot"/> can be used to store the database state. <see cref="ReportChanges"/> can be used to generate a difference report which can be
    /// verified.
    /// <para/>
    /// Note that once a snapshot is taken, the configuration cannot be changed further. Additionally, once calls are made to <see cref="DefineTable"/>, it is no longer possible to load schema
    /// information or overrides. This is because the schema configuration and overrides are merged in order to allow <see cref="DefineTable"/> to change the configuration, and this can only
    /// be done once.
    /// </summary>
    public class SqlSnapshotCollection
    {
        private readonly string _connectionString;
        private object _lock = new object();
        private SnapshotCollection _collection;
        private readonly Dictionary<string, SchemaStructure> _schemas = new Dictionary<string, SchemaStructure>();
        private readonly List<DefinitionSet> _overrides = new List<DefinitionSet>();
        private bool _snapshotTaken;
        private bool _collectionConfigured;
        
        public SqlSnapshotCollection(string connectionString)
        {
            _connectionString = connectionString;
            _collection = new SnapshotCollection();
            _collection.SetTimeSource(new SQLTimeSource(_connectionString));
        }

        public void ConfigureSchema(string schema)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
            _schemas[schema] = SchemaStructureLoader.Load(connectionStringBuilder.DataSource, connectionStringBuilder.InitialCatalog, schema);
        }

        public SnapshotBuilder Snapshot(string snapshotName, SnapshotOptions snapshotOpts = SnapshotOptions.Default)
        {
            lock (_lock)
            {
                ConfigureCollection();
                var builder = _collection.NewSnapshot(snapshotName);
                if ((snapshotOpts & SnapshotOptions.NoAutoSnapshot) == 0)
                    DbSnapshotMaker.Make(_connectionString, builder, _schemas.Values, _collection);
                _snapshotTaken = true;
                return builder;
            }
        }
        private void ConfigureCollection()
        {
            if (_collectionConfigured) return;
            
            _collectionConfigured = true;

            ApplyConfig();
        }

        private void ApplyConfig()
        {
            foreach (var schema in _schemas.Values)
            {
                SnapshotTableDefiner.Define(_collection, schema);
            }

            foreach (var definitionSet in _overrides)
            {
                _collection.ApplyDefinitions(definitionSet);
            }
        }

        public void ReportChanges(string before, string after, Output output, ChangeReportOptions changeReportOptions = ChangeReportOptions.Default)
        {
            lock (_lock)
            {
                ConfigureCollection();
                _collection.ReportChanges(before, after, output, changeReportOptions);
            }
        }

        public void GetSnapshotReport(string name, Output output, params string[] tables)
        {
            lock (_lock)
            {
                ConfigureCollection();
                _collection.GetSnapshotReport(name, output, tables);
            }
        }

        public TableDefiner DefineTable(string tableName)
        {
            lock (_lock)
            {
                if (_snapshotTaken)
                    throw new ConfigurationCannotBeChangedException();

                if (!_collectionConfigured)
                    ConfigureCollection();
                return _collection.DefineTable(tableName);
            }
        }

        public void GetSchemaReport(Output output, bool verbose = false)
        {
            lock (_lock)
            {
                ConfigureCollection();
                _collection.GetSchemaReport(output, verbose);
            }
        }

        public void LoadSchemaOverrides(Type containerType)
        {
            lock (_lock)
            {
                if (_collectionConfigured)
                    throw new ConfigurationCannotBeChangedException();
                _overrides.Add(SnapshotDefinitionLoader.Load(containerType));
            }
        }

        public void LoadSchemaOverrides(Assembly assembly)
        {
            lock (_lock)
            {
                if (_collectionConfigured)
                    throw new ConfigurationCannotBeChangedException();
                _overrides.Add(SnapshotDefinitionLoader.Load(assembly));
            }
        }
    }
}
