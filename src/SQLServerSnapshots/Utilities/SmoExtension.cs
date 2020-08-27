using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;

namespace SQLServerSnapshots.Utilities
{
    internal static class SmoExtension
    {
        internal static IEnumerable<Table> Tables(this Database database)
        {
            var count = database.Tables.Count;
            for (var i = 0; i < count; ++i)
                yield return database.Tables[i];
        }

        internal static IEnumerable<Column> Columns(this Table table)
        {
            var count = table.Columns.Count;
            for (var i = 0; i < count; ++i)
                yield return table.Columns[i];
        }

        internal static IEnumerable<ForeignKeyColumn> Columns(this ForeignKey fkey)
        {
            var cols = fkey.Columns;
            var count = cols.Count;
            for (var i = 0; i < count; ++i)
                yield return cols[i];
        }

        internal static IEnumerable<Schema> Schemas(this Database database)
        {
            var count = database.Schemas.Count;
            for (var i = 0; i < count; ++i)
                yield return database.Schemas[i];
        }

        internal static IEnumerable<ForeignKey> ForeignKeys(this Table table)
        {
            var count = table.ForeignKeys.Count;
            for (var i = 0; i < count; ++i)
                yield return table.ForeignKeys[i];
        }

    }
}
