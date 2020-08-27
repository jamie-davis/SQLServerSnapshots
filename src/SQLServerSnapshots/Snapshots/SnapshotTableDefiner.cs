﻿using System.Linq;
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
                }
            }
        }
    }
}