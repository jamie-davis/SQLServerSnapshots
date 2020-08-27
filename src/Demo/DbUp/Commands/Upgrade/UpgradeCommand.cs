using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace DbUp.Commands.Upgrade
{
    [Command]
    [Description("Upgrade command")]
    class UpgradeCommand
    {
        [Positional]
        [Description("The connection string to the database to be upgraded. This connection string must enable schema changes")]
        public string ConnectionString { get; set; }

        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {
            var upgrader = DeployChanges.To
                .SqlDatabase(ConnectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();
        }
    }
}
