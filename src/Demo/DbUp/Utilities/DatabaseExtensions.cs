using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Smo;

namespace DbUp.Utilities
{
    internal static class DatabaseExtensions
    {
        internal static IEnumerable<Database> Enumerate(this DatabaseCollection databases)
        {
            var count = databases.Count;
            for (var i = 0; i < count; ++i)
                yield return databases[i];
        }
    }
}
