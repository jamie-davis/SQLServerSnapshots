using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;
using TestConsole.OutputFormatting;
using TestConsoleLib;

namespace SQLServerSnapshots.Schemas
{
    internal class SchemaStructure
    {
        public SchemaStructure(string schema, Database database, List<TableStructure> tableDetails)
        {
            Name = database.Name;
            Schema = schema;
            Tables = tableDetails;
        }

        public string Schema { get; }
        public List<TableStructure> Tables { get; }

        public string Name { get; }

        public void Report(Output output)
        {
            var report = Tables.AsReport(rep => rep
                .Title($"[{Name}].[{Schema}]")
                .RemoveBufferLimit()
                .AddColumn(t => t.Name, cc => cc.Heading("Table Name"))
                .AddChild(t => t.Columns, 
                    trep => trep
                        .RemoveBufferLimit()
                        .AddColumn(c => c.Name)
                        .AddColumn(c => c.InPrimaryKey)
                )
                .AddChild(c => c.References, refs => refs.RemoveBufferLimit()
                    .AddColumn(r => string.Join(", ", r.ReferencingColumnNames.Select(c => $"[{c}]")), cc => cc.Heading("Referencing Columns"))
                    .AddColumn(r => $"[{r.ReferencedSchema}].[{r.ReferencedTable}]", cc => cc.Heading("Table"))
                    .AddColumn(r => string.Join(", ", r.ReferencedColumnNames.Select(c => $"[{c}]")), cc => cc.Heading("Referenced Columns"))
                )

            );
            output.FormatTable(report);
        }
    }
}