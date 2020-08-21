using System;

namespace Faraday.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FaradayPartitionKeyAttribute : Attribute
    {
        public string PartitionKeyAttributeName { get; }

        public FaradayPartitionKeyAttribute(string partitionKeyAttributeName = FaradayConstants.PartitionKeyName)
        {
            PartitionKeyAttributeName = partitionKeyAttributeName;
        }
    }
}
