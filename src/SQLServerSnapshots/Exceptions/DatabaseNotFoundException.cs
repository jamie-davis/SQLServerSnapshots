using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerSnapshots.Exceptions
{
    public class DatabaseNotFoundException : Exception
    {
        public string Server { get; }
        public string Database { get; }

        public DatabaseNotFoundException(string server, string database) : base("Database not found on server")
        {
            Server = server;
            Database = database;
        }
    }
}
