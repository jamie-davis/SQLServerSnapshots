using System;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using SqlDataType = Microsoft.SqlServer.Management.Smo.SqlDataType;

namespace SQLServerSnapshots.Schemas
{
    internal static class UnpredictableColumnDetector
    {
        public static (bool IsUnpredictable, bool IsUtcDateTime, bool IsLocalDateTime) IsUnpredictable(Column column)
        {
            switch (column.DataType.SqlDataType)
            {
                case SqlDataType.DateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.Date:
                case SqlDataType.Time:
                case SqlDataType.DateTimeOffset:
                    if (column.DefaultConstraint != null)
                    {
                        return IsCurrentTimeDefault(column.DefaultConstraint);
                    }

                    break;
            }

            var isUnpredictable = column.Identity || column.DataType.SqlDataType == SqlDataType.UniqueIdentifier;
            return (isUnpredictable, false, false);
        }

        private static (bool IsUnpredictable, bool IsUtcDateTime, bool IsLocalDateTime) IsCurrentTimeDefault(DefaultConstraint constraint)
        {
            var text = constraint.Text;
            switch (text)
            {
                case "(getutcdate())":
                case "(sysutcdatetime())":
                    return (true, true, false);

                case "(getdate())":
                case "(sysdatetime())":
                case "(sysdatetimeoffset())":
                    return (true, false, true);

                default:
                    return (false, false, false);
            }
        }
    }
}