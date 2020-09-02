using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.SqlServer.Management.Smo;
using SnapshotTests;
using SnapshotTests.Snapshots;
using SQLServerSnapshots.Schemas;
using SQLServerSnapshots.Snapshots;
using SQLServerSnapshots.Tests.Configuration;
using Xunit;

namespace SQLServerSnapshots.Tests.Snapshots
{
    public class TestSnapshotTableSelectBuilder
    {
        #region Definitions

        [SnapshotDefinition("[Test].[B_Related]")]
        [CustomWhereClause(@"WHERE [Name] LIKE 'First%'")]
        public static class B_RelatedConfig
        {

        }

        #endregion

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
";


        public TestSnapshotTableSelectBuilder()
        {
            DbController.TearDown();
            DbController.Run(CreateRelatedTables);
        }

        [Fact]
        public void BasicSelectIsBuilt()
        {
            //Arrange
            var collection = new SnapshotCollection();
            var schema = SchemaStructureLoader.Load(DbController.Server, DbController.TestDbName, "Test");
            SnapshotTableDefiner.Define(collection, schema);
            var definitions = SnapshotDefinitionLoader.Load(GetType());
            collection.ApplyDefinitions(definitions);
            var table = schema.Tables.First();
            
            //Act
            var select = SnapshotTableSelectBuilder.Build(table, collection.GetTableDefinition(table.Name));

            //Assert
            select.Should().Be("SELECT * FROM [Test].[A_Main]");
        }

        [Fact]
        public void WhereClauseIsLocatedAndApplied()
        {
            //Arrange
            var collection = new SnapshotCollection();
            var schema = SchemaStructureLoader.Load(DbController.Server, DbController.TestDbName, "Test");
            SnapshotTableDefiner.Define(collection, schema);
            var definitions = SnapshotDefinitionLoader.Load(GetType());
            collection.ApplyDefinitions(definitions);
            var table = schema.Tables.Single(t => t.Name == "[Test].[B_Related]");
            
            //Act
            var select = SnapshotTableSelectBuilder.Build(table, collection.GetTableDefinition(table.Name));

            //Assert
            select.Should().Be("SELECT * FROM [Test].[B_Related] WHERE [Name] LIKE 'First%'");
        }
    }
}
