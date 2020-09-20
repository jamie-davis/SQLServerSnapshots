using System;

namespace SQLServerSnapshots.Exceptions
{
    public class UnableToGetDateTimeException : Exception
    {
        public UnableToGetDateTimeException() : base("Unable to determine the current date/time")
        {
            
        }
    }
}
