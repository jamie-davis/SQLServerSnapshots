using System;
using System.Data;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;
using SQLServerSnapshots.Exceptions;
using SQLServerSnapshots.Utilities;

namespace SQLServerSnapshots.Schemas
{
    internal class SchemaStructureLoader
    {
        public static SchemaStructure Load(string server, string databaseName, string schema)
        {
            var srv = new Server(server);
            var database = srv.Databases[databaseName];
            if (database == null) 
                throw new DatabaseNotFoundException(server, databaseName);

            var tableDetails = database.Tables()
                .Where(t => !t.IsSystemObject && t.Schema == schema)
                .Select(GetTableDetails)
                .ToList();

            return new SchemaStructure(schema, database, tableDetails);   
        }

        private static TableStructure GetTableDetails(Table table)
        {
            var columns = table.Columns().Select(GetColumnDetails).ToList();

            var foreignKeys = table.ForeignKeys().Select(f => new Reference(f.Columns().Select(c => c.Name), f.ReferencedTableSchema, f.ReferencedTable, f.Columns().Select(c => c.Name)));
            
            return new TableStructure(table, columns, foreignKeys);
        }

        private static ColumnInfo GetColumnDetails(Column column)
        {
            return new ColumnInfo(column);
        }
    }
}
