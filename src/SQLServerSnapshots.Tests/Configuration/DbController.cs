using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using SQLServerSnapshots.Schemas;
using SQLServerSnapshots.Utilities;

namespace SQLServerSnapshots.Tests.Configuration
{
    internal static class DbController
    {
        private static Database _database;
        internal const string TestDbName = "SqlServerSnapshots.Tests";
        internal const string Server = "localhost";

        /// <summary>
        /// Ensure that every test run starts with a fresh database.
        /// </summary>
        static DbController()
        {
            var srv = new Server(Server);
            var db = AllDb(srv.Databases).FirstOrDefault(d => d.Name == TestDbName);

            db?.Drop();
            _database = new Database(srv, TestDbName);
            _database.Create();
            ConnectionString = $"Server=localhost;Database={TestDbName};Trusted_Connection=True;";
        }

        /// <summary>
        /// A connection string for the test database.
        /// </summary>
        public static string ConnectionString { get; }

        /// <summary>
        /// This method tears down any tables etc present in the database from a previous test.
        /// </summary>
        public static void TearDown()
        {
            var srv = new Server(Server);
            var db = AllDb(srv.Databases).FirstOrDefault(d => d.Name == TestDbName);
            if (db == null)
                throw new Exception($"{TestDbName} not found");

            var tables = db.Tables().OrderByDescending(t => t.Name).ToList();
            foreach (var table in tables)
            {
                table.Drop();
            }

            var schemas = db.Schemas()
                .Where(s => !s.IsSystemObject)
                .ToList();
            foreach (Schema dbSchema in schemas)
            {
                dbSchema.Drop();
            }
        }

        internal static IEnumerable<Database> AllDb(DatabaseCollection databases)
        {
            var count = databases.Count;
            for (var i = 0; i < count; ++i)
                yield return databases[i];
        }

        public static void Run(string sql)
        {
            foreach (var statement in GetStatements(sql))
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(statement, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static IEnumerable<string> GetStatements(string sql)
        {
            using (var reader = new StringReader(sql))
            {
                var sb = new StringBuilder();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim() == "GO")
                    {
                        if (sb.Length > 0)
                            yield return sb.ToString();
                        sb = new StringBuilder();
                    }
                    else
                    {
                        if (sb.Length > 0 || !string.IsNullOrEmpty(line))
                            sb.AppendLine(line);
                    }
                }

                if (sb.Length > 0)
                    yield return sb.ToString();
            }
        }
    }
}
