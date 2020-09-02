using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerSnapshots
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomWhereClauseAttribute : Attribute
    {
        public string WhereClause { get; }

        public CustomWhereClauseAttribute(string whereClause)
        {
            WhereClause = whereClause;
        }
    }
}
