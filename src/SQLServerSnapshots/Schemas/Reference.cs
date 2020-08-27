using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;

namespace SQLServerSnapshots.Schemas
{
    internal class Reference
    {
        public List<string> ReferencingColumnNames { get; }
        public string ReferencedSchema { get; }
        public string ReferencedTable { get; }
        public List<string> ReferencedColumnNames { get; }

        public Reference(IEnumerable<string> referencingColumns, string referencedSchema,
            string referencedTable, IEnumerable<string> referencedColumnNames)
        {
            ReferencingColumnNames = referencingColumns.ToList();
            ReferencedSchema = referencedSchema;
            ReferencedTable = referencedTable;
            ReferencedColumnNames = referencedColumnNames.ToList();
        }
    }
}