using System;
using Microsoft.Data.SqlClient;
using SnapshotTests;
using SQLServerSnapshots.Exceptions;
using SQLServerSnapshots.Snapshots;

namespace SQLServerSnapshots.Utilities
{
    internal class SQLTimeSource : ITimeSource
    {
        private readonly string _connectionString;
        private bool _firstRequest = true;
        public SQLTimeSource(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Implementation of ITimeSource

        public DateTime GetUtcTime()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT GETUTCDATE()"))
                {
                    command.Connection = conn;
                    conn.Open();
                    var result = command.ExecuteScalar();
                    if (result is DateTime dateTime)
                        return dateTime;

                    throw new UnableToGetDateTimeException();
                }
            }
        }

        #endregion
    }
}