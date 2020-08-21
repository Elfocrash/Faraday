using System;

namespace Faraday.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FaradaySharedTableAttribute : Attribute
    {
        public string TableName { get; }

        public string SharedTypeNameOverride { get; }

        public FaradaySharedTableAttribute(string tableName, string sharedTypeNameOverride = null)
        {
            TableName = tableName;
            SharedTypeNameOverride = sharedTypeNameOverride;
        }
    }
}
