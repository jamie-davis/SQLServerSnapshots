using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using DbTests.DataSetup;
using DbTests.TestConfiguration;
using DemoDbLib.Database;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Respawn;
using SnapshotTests;
using SQLServerSnapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;
using Xunit;

namespace DbTests
{
    public class AddPlayerTest
    {
        private ChessObjectMother _om;

        public AddPlayerTest()
        {
            _om = new ChessObjectMother();
            _om.ResetAsync().Wait();
        }

        [Fact]
        public async void DatabaseChangesAreFlagged()
        {
            //Arrange
            var collection = new SqlSnapshotCollection();
            collection.ConfigureSchema(Config.Server, Config.Database, "Chess");

            var builder = collection.Snapshot(Config.ConnectionString, "before");

            var entities = new ChessContext(Config.ConnectionString);

            //Act
            await _om.AddPlayerAsync("Bill", (ChessObjectMother.ChessDotCom, 1801), (ChessObjectMother.Lichess, 1992));
            await _om.AddPlayerAsync("Ted", (ChessObjectMother.ChessDotCom, 1836), (ChessObjectMother.Lichess, 1918));
            collection.Snapshot(Config.ConnectionString, "after");

            //Assert
            var output = new Output();
            collection.GetSchemaReport(output);
            collection.ReportChanges("before", "after", output);
            output.Report.Verify();
        }

    }
}
