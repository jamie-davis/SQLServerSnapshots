using System;
using System.Collections.Generic;
using System.Text;
using DbTests.TestConfiguration;
using SQLServerSnapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;
using Xunit;

namespace DbTests
{
    public class TestSchema
    {
        [Fact]
        public void SchemaConfigurationIsConsistent()
        {
            //Arrange
            var collection = new SqlSnapshotCollection(Config.ConnectionString);
            
            //Act
            collection.ConfigureSchema("Chess"); //this loads table definitions direct from the database schema. In theory no further config should be required

            //Assert
            var output = new Output();
            output.WrapLine("Default schema");
            output.WriteLine();
            collection.GetSchemaReport(output, true);            
            output.Report.Verify();
        }

        [Fact]
        public void SchemaConfigurationIsRefined()
        {
            //Arrange/Act
            var collection = SchemaObjectMother.MakeCollection();

            //Assert
            var output = new Output();
            output.WrapLine("Refined schema");
            output.WriteLine();
            collection.GetSchemaReport(output, true);            
            output.Report.Verify();
        }
    }
}
