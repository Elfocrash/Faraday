using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal;

namespace Faraday
{
    public class DynamoStore<TEntity> : IDynamoStore<TEntity> where TEntity : class
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly string _tableName;
        private readonly bool _isSharedTable;
        private readonly string _sharedTableTypeName;
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly Func<TEntity, string> _partitionKeyDescriptor;
        private readonly Func<TEntity, string> _sortKeyDescriptor;
        private readonly string _partitionKeyAttributeName;
        private readonly string _sortKeyAttributeName;

        public DynamoStore(
            IAmazonDynamoDB amazonDynamoDb,
            string tableName,
            Func<TEntity, string> partitionKeyDescriptor,
            string partitionKeyAttributeName = FaradayConstants.PartitionKeyName,
            Func<TEntity, string> sortKeyDescriptor = null,
            string sortKeyAttributeName = FaradayConstants.SortKeyName,
            bool isSharedTable = false,
            string sharedTableTypeNameOverride = null
            )
        {
            _amazonDynamoDb = amazonDynamoDb;
            _tableName = tableName;
            _isSharedTable = isSharedTable;
            _sharedTableTypeName = sharedTableTypeNameOverride ?? typeof(TEntity).Name;
            _partitionKeyDescriptor = partitionKeyDescriptor;
            _partitionKeyAttributeName = partitionKeyAttributeName;
            _sortKeyDescriptor = sortKeyDescriptor;
            _sortKeyAttributeName = sortKeyAttributeName;
            _dynamoDbContext = new DynamoDBContext(_amazonDynamoDb);
        }

        public DynamoStore(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
            var entityType = typeof(TEntity);
            _tableName = EntityTypeInfoResolver.ResolveTableName(entityType);
            _partitionKeyDescriptor = EntityTypeInfoResolver.ResolvePartitionKeyDescriptor<TEntity>(entityType);
            _partitionKeyAttributeName = EntityTypeInfoResolver.ResolvePartitionKeyAttributeName(entityType);
            _sortKeyDescriptor = EntityTypeInfoResolver.ResolveSortKeyDescriptor<TEntity>(entityType);
            _sortKeyAttributeName = EntityTypeInfoResolver.ResolveSortKeyAttributeName(entityType);
            _isSharedTable = EntityTypeInfoResolver.ResolveIsSharedTable(entityType);
            _sharedTableTypeName = EntityTypeInfoResolver.ResolveSharedTableTypeNameOverride(entityType) ?? entityType.Name;
            _dynamoDbContext = new DynamoDBContext(_amazonDynamoDb);
        }

        public async Task UpsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var document = _dynamoDbContext.ToDocument(entity);
            var attributeMap = document.ToAttributeMap();
            attributeMap[_partitionKeyAttributeName] = new AttributeValue(_partitionKeyDescriptor(entity));

            if (_sortKeyDescriptor != null)
            {
                attributeMap[_sortKeyAttributeName] = new AttributeValue(_sortKeyDescriptor(entity));
            }

            attributeMap[FaradayConstants.TypeKeyName] = new AttributeValue(_sharedTableTypeName);

            var putItemResponse = await _amazonDynamoDb.PutItemAsync(_tableName, attributeMap, cancellationToken);
        }

        public async Task<TEntity> GetAsync(string partitionKey, string sortKey = null, CancellationToken cancellationToken = default)
        {
            var attributeMap = new Dictionary<string, AttributeValue>
            {
                {_partitionKeyAttributeName, new AttributeValue(partitionKey)}
            };

            if (!string.IsNullOrEmpty(sortKey))
            {
                attributeMap[_sortKeyAttributeName] = new AttributeValue(sortKey);
            }

            var response = await _amazonDynamoDb.GetItemAsync(_tableName, attributeMap, cancellationToken);

            if (!response.IsItemSet)
            {
                return null;
            }

            var document = Document.FromAttributeMap(response.Item);
            return _dynamoDbContext.FromDocument<TEntity>(document);
        }

        public async Task DeleteAsync(string partitionKey, string sortKey = null, CancellationToken cancellationToken = default)
        {
            var attributeMap = new Dictionary<string, AttributeValue>
            {
                {_partitionKeyAttributeName, new AttributeValue(partitionKey)}
            };

            if (!string.IsNullOrEmpty(sortKey))
            {
                attributeMap[_sortKeyAttributeName] = new AttributeValue(sortKey);
            }

            var deleteItemResponse = await _amazonDynamoDb.DeleteItemAsync(_tableName, attributeMap, cancellationToken);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(_partitionKeyDescriptor(entity), _sortKeyDescriptor?.Invoke(entity), cancellationToken);
        }
    }
}
