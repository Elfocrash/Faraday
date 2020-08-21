using System;

namespace Faraday.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FaradaySortKeyAttribute : Attribute
    {
        public string SortKeyAttributeName { get; }

        public FaradaySortKeyAttribute(string sortKeyAttributeName = FaradayConstants.SortKeyName)
        {
            SortKeyAttributeName = sortKeyAttributeName;
        }
    }
}
