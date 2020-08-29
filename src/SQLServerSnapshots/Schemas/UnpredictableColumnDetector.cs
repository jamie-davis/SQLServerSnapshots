using System;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using SqlDataType = Microsoft.SqlServer.Management.Smo.SqlDataType;

namespace SQLServerSnapshots.Schemas
{
    internal static class UnpredictableColumnDetector
    {
        public static bool IsUnpredictable(Column column)
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
                        return IsCurrentTimeDefaul(column.DefaultConstraint);
                    }

                    break;
            }
            return column.Identity || column.DataType.SqlDataType == SqlDataType.UniqueIdentifier;
        }

        private static bool IsCurrentTimeDefaul(DefaultConstraint constraint)
        {
            var text = constraint.Text;
            switch (text)
            {
                case "(getdate())":
                case "(getutcdate())":
                case "(sysdatetime())":
                case "(sysutcdatetime())":
                case "(sysdatetimeoffset())":
                    return true;

                default:
                    return false;
            }
        }
    }
}