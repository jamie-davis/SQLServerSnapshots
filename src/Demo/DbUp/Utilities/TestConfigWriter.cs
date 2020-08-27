using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Xml.Linq;

namespace DbUp.Utilities
{
    internal static class TestConfigWriter
    {
        /// <summary>
        /// Write the testing configuration file and return the path to the file.
        /// </summary>
        /// <param name="connectionString">The connection string to embed in the configuration</param>
        /// <param name="bacpacFile"></param>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <returns>The path to the file that was generated.</returns>
        public static (string GeneratedFilePath, string Error) Write(string connectionString, string server, string database)
        {
            var fileLocation = ConfigLocator.LocateConfigPath();
            if (string.IsNullOrEmpty(fileLocation))
                return (null, $"Unable to determine location for testing configuration file.");

            var filePath = Path.Combine(fileLocation, "Testing.Config.xml");
            var xDoc = new XDocument(new XElement("testconfig"
                , new XElement("connectionstring", connectionString)
                , new XElement("server", server)
                , new XElement("database", database)
                ));
            xDoc.Save(filePath);
            return (filePath, null);
        }

    }
}