using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerSnapshots.Exceptions
{
    public class ConfigurationCannotBeChangedException : Exception
    {
        public ConfigurationCannotBeChangedException() : base("Once the configuration has been used, it can no longer be changed. Ensure all schema and definition updates are complete before taking or using snapshots.")
        {
            
        }
    }
}
