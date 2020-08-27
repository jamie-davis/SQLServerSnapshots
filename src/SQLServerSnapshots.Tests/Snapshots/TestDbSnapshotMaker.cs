﻿using SnapshotTests;
using SQLServerSnapshots.Schemas;
using SQLServerSnapshots.Snapshots;
using SQLServerSnapshots.Tests.Configuration;
using SQLServerSnapshots.Tests.Schemas;
using TestConsoleLib;
using TestConsoleLib.Testing;
using Xunit;
// ReSharper disable UnusedType.Global

namespace SQLServerSnapshots.Tests.Snapshots
{
    public class TestDbSnapshotMaker
    {
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
INSERT INTO [Test].[A_Main] (Name, CreatedDate) VALUES ('First', '2020-08-23 10:01')
GO
INSERT INTO [Test].[A_Main] (Name, CreatedDate) VALUES ('Second', '2020-08-23 10:05')
GO
INSERT INTO [Test].[B_Related] (Name, MainId)
SELECT 'First Sub 1', 1
UNION SELECT 'First Sub 2', 1
UNION SELECT 'First Sub 3', 1
UNION SELECT 'First Sub 4', 1
UNION SELECT 'Second Sub 1', 2
UNION SELECT 'Second Sub 2', 2
UNION SELECT 'Second Sub 3', 2
GO
";


        public TestDbSnapshotMaker()
        {
            DbController.TearDown();
            DbController.Run(CreateRelatedTables);
        }

        [Fact]
        public void BasicTableStructureIsLoaded()
        {
            //Arrange
            var collection = new SnapshotCollection();
            var schema = SchemaStructureLoader.Load(DbController.Server, DbController.TestDbName, "Test");
            var builder = collection.NewSnapshot("Test");
            SnapshotTableDefiner.Define(collection, schema);

            //Act
            DbSnapshotMaker.Make(DbController.ConnectionString, builder, new [] { schema });

            //Assert
            var output = new Output();
            collection.GetSnapshotReport("Test", output);
            output.Report.Verify();
        }
    }
}