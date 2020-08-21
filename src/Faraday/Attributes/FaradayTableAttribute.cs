using System;

namespace Faraday.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FaradayTableAttribute : Attribute
    {
        public string TableName { get; }

        public FaradayTableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
