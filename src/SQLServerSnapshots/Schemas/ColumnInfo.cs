using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.SqlServer.Management.Smo;

namespace SQLServerSnapshots.Schemas
{
    internal class ColumnInfo
    {
        public ColumnInfo(Column column)
        {
            Name = column.Name;
            InPrimaryKey = column.InPrimaryKey;
            (IsUnpredictable, IsUtcDateTime, IsLocalDateTime) = UnpredictableColumnDetector.IsUnpredictable(column);
        }

        internal string Name { get; }
        internal bool InPrimaryKey { get; }
        public bool IsUnpredictable { get; }
        public bool IsUtcDateTime { get; }
        public bool IsLocalDateTime { get; }
    }
}