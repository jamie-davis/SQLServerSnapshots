using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using SnapshotTests;
using SnapshotTests.Snapshots;
using SQLServerSnapshots.Schemas;

namespace SQLServerSnapshots.Snapshots
{
    internal static class DbSnapshotMaker
    {
        public static void Make(string connectionString, SnapshotBuilder builder, IEnumerable<SchemaStructure> schemas,
            SnapshotCollection snapshotCollection)
        {
            var schemasOrdered = schemas.OrderBy(s => s.Name);
            foreach (var schema in schemasOrdered)
            {
                foreach (var table in schema.Tables)
                {
                    var definition = snapshotCollection.GetTableDefinition(table.Name);
                    if (definition?.ExcludeFromComparison ?? false)
                        continue;

                    using (var conn = new SqlConnection(connectionString))
                    {
                        using (var command = new SqlCommand(SnapshotTableSelectBuilder.Build(table, definition)))
                        {
                            command.Connection = conn;
                            conn.Open();
                            Dictionary<string, int> columnIndex = null;
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    RowBuilder rowBuilder = null;
                                    foreach (var tableColumn in table.Columns)
                                    {
                                        if (columnIndex == null)
                                            columnIndex = LoadColumnIndex(reader);

                                        if (columnIndex.TryGetValue(tableColumn.Name, out var currentIx))
                                        {
                                            if (rowBuilder == null)
                                            {
                                                rowBuilder = builder.AddNewRow(table.Name);
                                            }

                                            rowBuilder[tableColumn.Name] = reader[currentIx];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static Dictionary<string, int> LoadColumnIndex(SqlDataReader reader)
        {
            var result = new Dictionary<string, int>();
            for (var ix = 0; ix < reader.VisibleFieldCount; ix++)
            {
                result[reader.GetName(ix)] = ix;
            }

            return result;
        }
    }
}