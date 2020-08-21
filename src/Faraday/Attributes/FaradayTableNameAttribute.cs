using System;

namespace Faraday.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FaradayTableNameAttribute : Attribute
    {
        public string TableName { get; }

        public FaradayTableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
