using System.Threading;
using System.Threading.Tasks;

namespace Faraday
{
    public interface IDynamoStore<TEntity> where TEntity : class
    {
        Task UpsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(string partitionKey, string sortKey = null, CancellationToken cancellationToken = default);

        Task DeleteAsync(string partitionKey, string sortKey = null, CancellationToken cancellationToken = default);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
