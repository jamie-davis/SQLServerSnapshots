// ReSharper disable UnusedType.Global

using SnapshotTests;
using SQLServerSnapshots.Schemas;
using SQLServerSnapshots.Snapshots;
using SQLServerSnapshots.Tests.Configuration;
using TestConsoleLib;
using TestConsoleLib.Testing;
using Xunit;

namespace SQLServerSnapshots.Tests.Snapshots
{
    public class TestSnapshotTableDefiner
    {
        private SchemaStructure _schema;

        private const string CreateRelatedTables = @"
CREATE SCHEMA Test;
GO
CREATE TABLE [Test].[A_Main]
(
    MainId INT PRIMARY KEY IDENTITY,
    Name VARCHAR(100) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT(GETDATE())
);
GO
CREATE TABLE [Test].[B_Related]
(
    BId INT PRIMARY KEY IDENTITY,
    Name VARCHAR(100) NULL,
    MainId INT NOT NULL,
CONSTRAINT fk_B_MainId
    FOREIGN KEY (MainId)
    REFERENCES [Test].[A_Main] (MainId)
);
GO
";
    
        public TestSnapshotTableDefiner()
        {
            DbController.TearDown();
            DbController.Run(CreateRelatedTables);
            _schema = SchemaStructureLoader.Load(DbController.Server, DbController.TestDbName, "Test");
        }

        [Fact]
        public void BasicTableStructureIsLoaded()
        {
            //Arrange
            var collection = new SnapshotCollection();

            //Act
            SnapshotTableDefiner.Define(collection, _schema);

            //Assert
            var output = new Output();
            collection.GetSchemaReport(output);
            output.Report.Verify();
        }
    }
}