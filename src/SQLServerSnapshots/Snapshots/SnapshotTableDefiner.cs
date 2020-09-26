using System.Linq;
using SnapshotTests;
using SQLServerSnapshots.Schemas;

namespace SQLServerSnapshots.Snapshots
{
    /// <summary>
    /// Creates the snapshot tables for an extracted database schema.
    /// </summary>
    internal static class SnapshotTableDefiner
    {
        public static void Define(SnapshotCollection collection, SchemaStructure schema)
        {
            foreach (var table in schema.Tables)
            {
                var tableDef = collection.DefineTable(table.Name);
                foreach (var column in table.Columns.Where(c => c.InPrimaryKey))
                {
                    tableDef.PrimaryKey(column.Name);
                    if (column.IsUnpredictable)
                        tableDef.IsUnpredictable(column.Name);

                    if (column.IsUtcDateTime)
                        tableDef.Utc(column.Name);

                    if (column.IsLocalDateTime)
                        tableDef.Local(column.Name);
                }

                foreach (var reference in table.References)
                {
                    tableDef.IsReference(reference.ReferencingColumnNames.First(), $"[{reference.ReferencedSchema}].[{reference.ReferencedTable}]", reference.ReferencedColumnNames.First());
                }
            }
        }
    }
}