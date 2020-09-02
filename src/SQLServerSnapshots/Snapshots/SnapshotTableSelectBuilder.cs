using System;
using System.Linq;
using System.Reflection;
using SnapshotTests.Snapshots;
using SQLServerSnapshots.Schemas;

namespace SQLServerSnapshots.Snapshots
{
    internal static class SnapshotTableSelectBuilder
    {
        internal static string Build(TableStructure table, TableDefinition tableDefinition)
        {
            var select = $"SELECT * FROM {table.Name}";

            var whereSource = tableDefinition?.DefiningTypes.Reverse()
                .Select(t => t.GetCustomAttribute<CustomWhereClauseAttribute>()).FirstOrDefault();
            if (whereSource != null)
                select = AddWhere(select, whereSource);
            return select;
        }

        private static string AddWhere(string select, CustomWhereClauseAttribute whereSource)
        {
            var clause = whereSource.WhereClause.Trim();
            if (!clause.StartsWith("where ", StringComparison.InvariantCultureIgnoreCase))
                clause = $"where {clause}";

            return $"{select} {clause}";
        }
    }
}