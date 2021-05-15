namespace KO.Covid.Infrastructure.Cosmos
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    public interface ICosmosDbClient
    {
        Task<ItemResponse<T>> ReadItemAsync<T>(
            string id,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> QueryItemsAsync<T>(
            Expression<Func<T, bool>> predicate,
            bool allowSynchronousQueryExecution = false,
            CancellationToken cancellationToken = default);

        Task<long> GetItemsCountAsync<T>(
            Expression<Func<T, bool>> predicate,
            bool allowSynchronousQueryExecution = false,
            CancellationToken cancellationToken = default);

        Task<ItemResponse<T>> CreateItemAsync<T>(
            T item, PartitionKey? partitionKey = null,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> QueryItemsAsync<T>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderByPredicate,
            int pageNumber,
            int pageSize,
            bool allowSynchronousQueryExecution = false,
            CancellationToken cancellationToken = default);

        Task<ItemResponse<T>> ReplaceItemAsync<T>(
            string id,
            T item,
            PartitionKey? partitionKey = null,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default);

        Task<ItemResponse<T>> UpsertItemAsync<T>(
            T item,
            PartitionKey? partitionKey = null,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default);

        Task<ItemResponse<T>> DeleteItemAsync<T>(
            string id,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default);
    }
}
