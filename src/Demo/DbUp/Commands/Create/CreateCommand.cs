using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using DbUp.Commands.Upgrade;
using DbUp.Utilities;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Management.Smo;

namespace DbUp.Commands.Create
{
    [Command]
    [Description("Create command, used to generate an empty database for unit testing.")]
    class CreateCommand
    {
        [Positional(DefaultValue = "localhost")]
        [Description("The server on which the database is to be created. Defaults to localhost.")]
        public string Server { get; set; }

        [Positional(DefaultValue = "TestDb")]
        [Description("The name of the test database. Defaults to TestDb")]
        public string Database { get; set; }

        [Option("replace", "r")]
        [Description("If the database already exists it will be deleted and recreated.")]
        public bool Replace { get; set; }

        [Option("noconfig", "n")]
        [Description("Do not generate a debugging configuration file")]
        public bool NoConfig { get; set; }

        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {
            //Connect to the local, default instance of SQL Server.   
            var srv = new Server(Server);   
            ReportDatabases(console, srv);

            var db = srv.Databases.Enumerate().SingleOrDefault(d => d.Name == Database);
            if (db != null)
            {
                if (!Replace && !console.Confirm($"Drop {Database.Yellow()}?".Cyan()))
                {
                    error.WrapLine($"Database {Database.Yellow()} already exists. Specify -r to replace it.".Red());
                    Environment.ExitCode = -100;
                    return;
                }

                console.WrapLine($"Dropping {Database.Yellow()}".Cyan());
                db.Drop();
            }

            console.WrapLine($"Creating {Database.Yellow()}".Cyan());
            var database = new Database(srv, Database);
            database.Create();

            var connectionString = $"Server={Server};Database={Database};Trusted_Connection=True;";
            var upgradeCommand = UpgradeDb(console, error, connectionString);

            if (!NoConfig) GenerateTestingConfig(console, error, connectionString);
        }

        private static UpgradeCommand UpgradeDb(IConsoleAdapter console, IErrorAdapter error, string connectionString)
        {
            var upgradeCommand = new UpgradeCommand()
            {
                ConnectionString = connectionString
            };
            upgradeCommand.Handle(console, error);
            return upgradeCommand;
        }

        private void ReportDatabases(IConsoleAdapter console, Server srv)
        {
            console.FormatTable(new[] {new {Server = Server, Version = srv.Information.Version}});
            console.WriteLine();
            console.WrapLine("Databases");
            console.WriteLine();
            var databases = srv.Databases.Enumerate()
                .Where(d => !d.IsSystemObject)
                .Select(d => new {d.Name, d.CreateDate, Size = $"{d.Size} MB"})
                .OrderBy(d => d.CreateDate);
            console.FormatTable(databases);
            console.WriteLine();
        }

        private void GenerateTestingConfig(IConsoleAdapter console, IErrorAdapter error, string connectionString)
        {
            console.WrapLine($"Creating testing configuration file".Cyan());
            var (configFile, configError) = TestConfigWriter.Write(connectionString, Server, Database);
            if (configError == null)
                console.WrapLine($"{configFile.Yellow()} created.".Cyan());
            else
            {
                error.WrapLine(configError.Red());
                return;
            }

        }
    }
}
