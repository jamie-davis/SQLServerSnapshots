using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;
using SnapshotTests;

namespace SQLServerSnapshots.Schemas
{
    internal class TableStructure
    {
        public TableStructure(Table table, List<ColumnInfo> columns, IEnumerable<Reference> foreignKeys)
        {
            Name = table.Name;
            Columns = columns;
            References = foreignKeys.ToList();
        }

        public List<Reference> References { get; }

        public List<ColumnInfo> Columns { get; }

        public string Name { get; }
    }
}