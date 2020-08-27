using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using SnapshotTests.Snapshots;
using SQLServerSnapshots.Schemas;

namespace SQLServerSnapshots.Snapshots
{
    internal static class DbSnapshotMaker
    {
        public static void Make(string connectionString, SnapshotBuilder builder, IEnumerable<SchemaStructure> schemas)
        {
            var schemasOrdered = schemas.OrderBy(s => s.Name);
            foreach (var schema in schemasOrdered)
            {
                foreach (var table in schema.Tables)
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        using (var command = new SqlCommand($"SELECT * FROM [{schema.Schema}].[{table.Name}]"))
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