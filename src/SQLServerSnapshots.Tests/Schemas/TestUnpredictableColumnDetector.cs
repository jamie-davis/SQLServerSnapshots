using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.SqlServer.Management.Smo;
using SQLServerSnapshots.Schemas;
using SQLServerSnapshots.Tests.Configuration;
using TestConsoleLib;
using TestConsoleLib.Testing;
using Xunit;
using SQLServerSnapshots.Utilities;

namespace SQLServerSnapshots.Tests.Schemas
{
    public class TestUnpredictableColumnDetector
    {
        [Category("localhost Dependent")]
        public class TestSchemaStructureLoader
        {
            private const string TableTemplate =
                @"
CREATE SCHEMA Test;
GO
CREATE TABLE [Test].[A]
(
    AKey {0},
    Value {1}
);
GO
";


            public TestSchemaStructureLoader()
            {
                DbController.TearDown();
            }

            private static Column GetColumn(string columnName)
            {
                var server = new Server(DbController.Server);
                var database = server.Databases[DbController.TestDbName];
                var table = database.Tables().First(t => t.Schema == "Test" && t.Name == "A");
                var column = table.Columns[columnName];
                return column;
            }

            private void CreateTable(string keyDef, string fieldDef)
            {
                var sql = string.Format(TableTemplate, keyDef ?? "INT IDENTITY PRIMARY KEY", fieldDef ?? "DATETIME");
                DbController.Run(sql);
            }

            [Fact]
            public void IdentityColumnIsUnpredictable()
            {
                //Arrange
                CreateTable("INT PRIMARY KEY IDENTITY", null);
                var column = GetColumn("AKey");

                //Act
                var result = UnpredictableColumnDetector.IsUnpredictable(column);

                //Assert
                result.Should().BeTrue();
            }

            [Fact]
            public void GuidColumnIsUnpredictable()
            {
                //Arrange
                CreateTable("UNIQUEIDENTIFIER PRIMARY KEY", null);
                var column = GetColumn("AKey");

                //Act
                var result = UnpredictableColumnDetector.IsUnpredictable(column);

                //Assert
                result.Should().BeTrue();
            }

            [Fact]
            public void NonIdentityColumnIsNotUnpredictable()
            {
                //Arrange
                CreateTable("INT PRIMARY KEY", null);
                var column = GetColumn("AKey");

                //Act
                var result = UnpredictableColumnDetector.IsUnpredictable(column);

                //Assert
                result.Should().BeFalse();
            }

            [Theory]
            [InlineData("DATETIME DEFAULT(GETDATE())", true)]
            [InlineData("DATETIME DEFAULT(GETUTCDATE())", true)]
            [InlineData("DATETIME DEFAULT(CURRENT_TIMESTAMP)", true)]
            [InlineData("DATETIME DEFAULT(SYSDATETIME())", true)]
            [InlineData("DATETIME DEFAULT(SYSDATETIMEOFFSET())", true)]
            [InlineData("DATETIME DEFAULT(SYSUTCDATETIME())", true)]
            [InlineData("DATETIME", false)]
            [InlineData("DATETIME2 DEFAULT(GETDATE())", true)]
            [InlineData("DATETIME2 DEFAULT(GETUTCDATE())", true)]
            [InlineData("DATETIME2 DEFAULT(CURRENT_TIMESTAMP)", true)]
            [InlineData("DATETIME2 DEFAULT(SYSDATETIME())", true)]
            [InlineData("DATETIME2 DEFAULT(SYSDATETIMEOFFSET())", true)]
            [InlineData("DATETIME2 DEFAULT(SYSUTCDATETIME())", true)]
            [InlineData("DATETIME2", false)]
            [InlineData("DATE DEFAULT(GETDATE())", true)]
            [InlineData("DATE DEFAULT(GETUTCDATE())", true)]
            [InlineData("DATE DEFAULT(CURRENT_TIMESTAMP)", true)]
            [InlineData("DATE DEFAULT(SYSDATETIME())", true)]
            [InlineData("DATE DEFAULT(SYSDATETIMEOFFSET())", true)]
            [InlineData("DATE DEFAULT(SYSUTCDATETIME())", true)]
            [InlineData("TIME DEFAULT(GETDATE())", true)]
            [InlineData("TIME DEFAULT(GETUTCDATE())", true)]
            [InlineData("TIME DEFAULT(CURRENT_TIMESTAMP)", true)]
            [InlineData("TIME DEFAULT(SYSDATETIME())", true)]
            [InlineData("TIME DEFAULT(SYSDATETIMEOFFSET())", true)]
            [InlineData("TIME DEFAULT(SYSUTCDATETIME())", true)]
            [InlineData("TIME", false)]
            [InlineData("DATETIMEOFFSET DEFAULT(GETDATE())", true)]
            [InlineData("DATETIMEOFFSET DEFAULT(GETUTCDATE())", true)]
            [InlineData("DATETIMEOFFSET DEFAULT(CURRENT_TIMESTAMP)", true)]
            [InlineData("DATETIMEOFFSET DEFAULT(SYSDATETIME())", true)]
            [InlineData("DATETIMEOFFSET DEFAULT(SYSDATETIMEOFFSET())", true)]
            [InlineData("DATETIMEOFFSET DEFAULT(SYSUTCDATETIME())", true)]
            [InlineData("DATETIMEOFFSET", false)]
            public void DefaultedDateValuesAreUnpredictable(string fieldDef, bool shouldBeUnpredictable)
            {
                //Arrange
                CreateTable(null, fieldDef);
                var column = GetColumn("Value");

                //Act
                var result = UnpredictableColumnDetector.IsUnpredictable(column);

                //Assert
                result.Should().Be(shouldBeUnpredictable);
            }

            [Fact]
            public void DateTimeWithoutDefaultIsNotUnpredictable()
            {
                //Arrange
                CreateTable(null, "DATETIME");
                var column = GetColumn("Value");

                //Act
                var result = UnpredictableColumnDetector.IsUnpredictable(column);

                //Assert
                result.Should().BeFalse();
            }
        }
    }
}
