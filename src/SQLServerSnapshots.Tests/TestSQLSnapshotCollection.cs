﻿using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using SnapshotTests;
using SQLServerSnapshots.Exceptions;
using SQLServerSnapshots.Schemas;
using SQLServerSnapshots.Snapshots;
using SQLServerSnapshots.Tests.Configuration;
using TestConsoleLib;
using TestConsoleLib.Testing;
using Xunit;

namespace SQLServerSnapshots.Tests
{
    public class TestSQLSnapshotCollection
    {
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
INSERT INTO [Test].[A_Main] (Name, CreatedDate)
VALUES ('Test Row', '2020-08-31 12:01')
GO
";
    
        #region Types configuring test schema overrides

        [SnapshotDefinition("[Test].[AMain]")]
        public static class AMain
        {
            [Predictable]
            public static int MainId { get; set; }
        }

        #endregion
        
        public TestSQLSnapshotCollection()
        {
            DbController.TearDown();
            DbController.Run(CreateRelatedTables);
        }

        [Fact]
        public void SchemaOverridesCanBeLoadedFromType()
        {
            //Arrange
            var collection = new SqlSnapshotCollection();
            collection.ConfigureSchema(DbController.Server, DbController.TestDbName, "Test");

            //Act
            collection.LoadSchemaOverrides(GetType());

            //Assert
            var output = new Output();
            collection.GetSchemaReport(output, true);
            output.Report.Verify();
        }

        [Fact]
        public void SchemaOverridesCanBeLoadedFromAssembly()
        {
            //Arrange
            var collection = new SqlSnapshotCollection();
            collection.ConfigureSchema(DbController.Server, DbController.TestDbName, "Test");

            //Act
            collection.LoadSchemaOverrides(GetType().Assembly);

            //Assert
            var output = new Output();
            collection.GetSchemaReport(output, true);
            output.Report.Verify();
        }

        [Fact]
        public void SchemaOverridesCannotBeAppliedAfterSnapshot()
        {
            //Arrange
            var collection = new SqlSnapshotCollection();
            collection.ConfigureSchema(DbController.Server, DbController.TestDbName, "Test");
            collection.Snapshot(DbController.ConnectionString, "Test");

            //Act
            Action action = () => collection.DefineTable("[Test].[A_Main]").IsPredictable("MainId");

            //Assert
            action.Should().Throw<ConfigurationCannotBeChangedException>();
        }

        [Fact]
        public void SchemaOverridesCannotBeLoadedAfterSnapshot()
        {
            //Arrange
            var collection = new SqlSnapshotCollection();
            collection.ConfigureSchema(DbController.Server, DbController.TestDbName, "Test");
            collection.Snapshot(DbController.ConnectionString, "Test");

            //Act
            Action action = () => collection.LoadSchemaOverrides(GetType());

            //Assert
            action.Should().Throw<ConfigurationCannotBeChangedException>();
        }

        [Fact]
        public void SchemaOverridesCannotBeLoadedFromAssemblyAfterSnapshot()
        {
            //Arrange
            var collection = new SqlSnapshotCollection();
            collection.ConfigureSchema(DbController.Server, DbController.TestDbName, "Test");
            collection.Snapshot(DbController.ConnectionString, "Test");

            //Act
            Action action = () => collection.LoadSchemaOverrides(GetType().Assembly);

            //Assert
            action.Should().Throw<ConfigurationCannotBeChangedException>();
        }

        [Fact]
        public void SnapshotReportCanBeExtracted()
        {
            //Arrange
            var collection = new SqlSnapshotCollection();
            collection.ConfigureSchema(DbController.Server, DbController.TestDbName, "Test");
            collection.Snapshot(DbController.ConnectionString, "Test");

            //Act
            var output = new Output();
            collection.GetSnapshotReport("Test", output);

            //Assert
            output.Report.Verify();
        }

        [Fact]
        public void SchemaOverridesCanBeAppliedDirectly()
        {
            //Arrange
            var collection = new SqlSnapshotCollection();
            collection.ConfigureSchema(DbController.Server, DbController.TestDbName, "Test");

            //Act
            collection.DefineTable("[Test].[A_Main]").IsPredictable("MainId");

            //Assert
            var output = new Output();
            collection.GetSchemaReport(output, true);
            output.Report.Verify();
        }
    }
}
