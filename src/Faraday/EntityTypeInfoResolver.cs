using System;
using System.Linq;
using System.Reflection;
using Faraday.Attributes;

namespace Faraday
{
    internal static class EntityTypeInfoResolver
    {
        //TODO create custom exceptions

        public static string ResolveTableName(Type entityType)
        {
            var tableNameAttribute = entityType.GetCustomAttribute<FaradayTableAttribute>();
            var sharedTableNameAttribute = entityType.GetCustomAttribute<FaradaySharedTableAttribute>();

            if (tableNameAttribute != null && sharedTableNameAttribute != null)
            {
                throw new Exception("You can only have either one FaradayTableAttribute or one FaradaySharedTableAttribute per entity");
            }

            var tableName = tableNameAttribute?.TableName ?? sharedTableNameAttribute?.TableName ??
                throw new Exception($"The entity of type {entityType.Name} is missing the FaradayTableNameAttribute or FaradaySharedTableAttribute");

            if (string.IsNullOrEmpty(tableName))
            {
                throw new Exception("The table name cannot be null or empty");
            }

            return tableName;
        }

        public static Func<TEntity, string> ResolvePartitionKeyDescriptor<TEntity>(Type entityType) where TEntity : class
        {
            var partitionKeyAttributes = entityType
                .GetProperties()
                .Where(x => x.GetCustomAttribute<FaradayPartitionKeyAttribute>() != null)
                .ToList();

            if (partitionKeyAttributes.Count == 0)
            {
                throw new Exception("You are required to have a property decorated with the FaradayPartitionKeyAttribute");
            }

            if (partitionKeyAttributes.Count > 1)
            {
                throw new Exception("You can only have one property decorated with the FaradayPartitionKeyAttribute");
            }

            var partitionKeyProperty = partitionKeyAttributes.Single();

            return entity => partitionKeyProperty.GetValue(entity).ToString();
        }

        public static string ResolvePartitionKeyAttributeName(Type entityType)
        {
            var partitionKeyAttributes = entityType
                .GetProperties()
                .Where(x => x.GetCustomAttribute<FaradayPartitionKeyAttribute>() != null)
                .ToList();

            if (partitionKeyAttributes.Count == 0)
            {
                throw new Exception("You are required to have a property decorated with the FaradayPartitionKeyAttribute");
            }

            if (partitionKeyAttributes.Count > 1)
            {
                throw new Exception("You can only have one property decorated with the FaradayPartitionKeyAttribute");
            }

            var partitionKeyProperty = partitionKeyAttributes.Single();

            var partitionKeyAttributeName = partitionKeyProperty.GetCustomAttribute<FaradayPartitionKeyAttribute>().PartitionKeyAttributeName;

            if (string.IsNullOrEmpty(partitionKeyAttributeName))
            {
                throw new Exception("The partition key cannot be null or empty");
            }

            return partitionKeyAttributeName;
        }

        public static Func<TEntity,string> ResolveSortKeyDescriptor<TEntity>(Type entityType) where TEntity : class
        {
            var sortKeyAttributes = entityType
                .GetProperties()
                .Where(x => x.GetCustomAttribute<FaradaySortKeyAttribute>() != null)
                .ToList();

            if (sortKeyAttributes.Count == 0)
            {
                return null;
            }

            if (sortKeyAttributes.Count > 1)
            {
                throw new Exception("You can only have one property decorated with the FaradaySortKeyAttribute");
            }

            var sortKeyProperty = sortKeyAttributes.Single();

            return entity => sortKeyProperty.GetValue(entity).ToString();
        }

        public static string ResolveSortKeyAttributeName(Type entityType)
        {
            var sortKeyAttributes = entityType
                .GetProperties()
                .Where(x => x.GetCustomAttribute<FaradaySortKeyAttribute>() != null)
                .ToList();

            if (sortKeyAttributes.Count == 0)
            {
                return null;
            }

            if (sortKeyAttributes.Count > 1)
            {
                throw new Exception("You can only have one property decorated with the FaradaySortKeyAttribute");
            }

            var sortKeyAttribute = sortKeyAttributes.Single().GetCustomAttribute<FaradaySortKeyAttribute>();

            var sortKeyAttributeName = sortKeyAttribute?.SortKeyAttributeName;

            if (sortKeyAttributeName == null)
            {
                return null;
            }

            if (sortKeyAttributeName.Length == 0)
            {
                throw new Exception("The sort key name cannot be an empty string");
            }

            return sortKeyAttributeName;
        }

        public static bool ResolveIsSharedTable(Type entityType)
        {
            var sharedTableAttribute = entityType.GetCustomAttribute<FaradaySharedTableAttribute>();
            return sharedTableAttribute != null;
        }

        public static string ResolveSharedTableTypeNameOverride(Type entityType)
        {
            var sharedTableAttribute = entityType.GetCustomAttribute<FaradaySharedTableAttribute>();

            if (sharedTableAttribute == null)
            {
                return null;
            }

            var sharedTableTypeNameOverride = sharedTableAttribute.SharedTypeNameOverride;

            if (sharedTableTypeNameOverride != null && sharedTableTypeNameOverride.Length == 0)
            {
                throw new Exception("SharedTypeNameOverride cannot be empty");
            }

            return sharedTableTypeNameOverride;
        }
    }
}
