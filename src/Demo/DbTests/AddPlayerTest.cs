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
        public async void DatabaseChangesAreFlaggedFluent()
        {
            //Arrange
            var collection = new SqlSnapshotCollection(Config.ConnectionString);
            collection.ConfigureSchema("Chess"); //this loads table definitions direct from the database schema. In theory no further config should be required

            //In practice, however, some extra configuration settings are needed
            collection.DefineTable("[Chess].[Rating]")
                .IsUnpredictable("RatingDate") //RatingDate is not defaulted to GETDATE() or any of the automatic "get the time" variants. This was deliberate, to illustrate that we can alter the automated decisions.
                .Sort("RatingDate")      //* In a similar vein, a GUID was used as the table key for [Chess].[Rating] to illustrate that GUIDs can be handled, however, when they are the primary key they can
                .Sort("RatingSourceId"); //* randomise the order rows are returned, so we need to force a sort on other fields to make the differences repeatable.

            collection.DefineTable("[Chess].[Player]")
                .Sort("Name"); // As above, we need to sort player data because it has a GUID for a primary key, randomising the order that rows are returned.

            collection.DefineTable("[Chess].[AuditPlayer]")
                .IsReference("PlayerId", "[Chess].[Player]", "PlayerId") //The audit table does not have the player ID defined as a foreign key, so no relationship is defined automatically. We define it manually to make the keys consistent.
                .IsUnpredictable("AuditStartDate") //* These dates will match the update timestamp. They are set by a trigger, not a default
                .IsUnpredictable("AuditEndDate")   //* otherwise unpredictability would have been set automatically.
                .Local("AuditStartDate")           //* And they are...
                .Local("AuditEndDate");            //* local dates

            collection.DefineTable("[Chess].[AuditRating]")
                .IsReference("PlayerId", "[Chess].[Player]", "PlayerId") //The audit table does not have the player ID defined as a foreign key, so no relationship is defined automatically. We define it manually to make the keys consistent.
                .IsReference("RatingId", "[Chess].[Rating]", "RatingId") //The audit table does not have the Rating Id defined as a foreign key, so no relationship is defined automatically. We define it manually to make the keys consistent.
                .IsUnpredictable("AuditStartDate") //* These dates will match the update timestamp. They are set by a trigger, not a default
                .IsUnpredictable("AuditEndDate")   //* otherwise unpredictability would have been set automatically.
                .IsUnpredictable("RatingDate")     //* 
                .Local("AuditStartDate")           //* And they are...
                .Local("AuditEndDate")             //* local dates
                .Local("RatingDate");              //*

            var builder = collection.Snapshot("before");

            var entities = new ChessContext(Config.ConnectionString);

            //Act
            await _om.AddPlayerAsync("Bill", (ChessObjectMother.ChessDotCom, 1801), (ChessObjectMother.Lichess, 1992));
            await _om.AddPlayerAsync("Ted", (ChessObjectMother.ChessDotCom, 1836), (ChessObjectMother.Lichess, 1918));
            collection.Snapshot("after");

            //Assert
            var output = new Output();
            collection.ReportChanges("before", "after", output);
            output.Report.Verify();
        }

        [Fact]
        public async void DatabaseChangesAreFlagged()
        {
            //Arrange
            var collection = SchemaObjectMother.MakeCollection();

            var entities = new ChessContext(Config.ConnectionString);
            var builder = collection.Snapshot("before");

            //Act
            await _om.AddPlayerAsync("Bill", (ChessObjectMother.ChessDotCom, 1801), (ChessObjectMother.Lichess, 1992));
            await _om.AddPlayerAsync("Ted", (ChessObjectMother.ChessDotCom, 1836), (ChessObjectMother.Lichess, 1918));
            collection.Snapshot("after");

            //Assert
            var output = new Output();
            collection.ReportChanges("before", "after", output);
            output.Report.Verify();
        }
    }
}
