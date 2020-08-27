using System.IO;
using System.Runtime.CompilerServices;

namespace DbUp.Utilities
{
    /// <summary>
    /// Determine where config data needs to be written within the test dll's folder structure.
    /// </summary>
    internal static class ConfigLocator
    {
        internal static string LocateConfigPath()
        {
            var fileLocation = GetFilePath();
            while ((fileLocation = Path.GetDirectoryName(fileLocation)) != null && Path.GetFileName(fileLocation) != "Demo");

            if (!string.IsNullOrEmpty(fileLocation))
            {
                fileLocation = Path.Combine(fileLocation, @"DbTests\TestConfiguration");
                if (!Directory.Exists(fileLocation))
                    return null;
            }

            return fileLocation;
        }

        private static string GetFilePath([CallerFilePath] string sourceFilePath = "")
        {
            return sourceFilePath;
        }
    }
}