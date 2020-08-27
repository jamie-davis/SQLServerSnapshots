using System.ComponentModel;
using SQLServerSnapshots.Schemas;
using SQLServerSnapshots.Tests.Configuration;
using TestConsoleLib;
using TestConsoleLib.Testing;
using Xunit;

namespace SQLServerSnapshots.Tests.Schemas
{
    [Category("localhost Dependent")]
    public class TestSchemaStructureLoader
    {
        private const string CreateTables =
            @"
CREATE SCHEMA Test;
GO
CREATE TABLE [Test].[A]
(
    Id INT PRIMARY KEY IDENTITY,
    Name VARCHAR(100) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT(GETDATE())
);
GO
";

        private const string CreateRelatedTables =
            @"
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
CREATE TABLE [C_Non_Schema]
(
    CId INT PRIMARY KEY IDENTITY,
    Name VARCHAR(100) NULL,
    MainId INT NOT NULL,
CONSTRAINT fk_B_MainId
    FOREIGN KEY (MainId)
    REFERENCES [Test].[A_Main] (MainId)
);
GO
";


        public TestSchemaStructureLoader()
        {
            DbController.TearDown();
        }

        [Fact]
        public void BasicTableStructureIsLoaded()
        {
            //Arrange
            DbController.Run(CreateTables);

            //Act
            var result = SchemaStructureLoader.Load(DbController.Server, DbController.TestDbName, "Test");

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }

        [Fact]
        public void RelationshipsAreExtracted()
        {
            //Arrange
            DbController.Run(CreateRelatedTables);

            //Act
            var result = SchemaStructureLoader.Load(DbController.Server, DbController.TestDbName, "Test");

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }
    }
}
