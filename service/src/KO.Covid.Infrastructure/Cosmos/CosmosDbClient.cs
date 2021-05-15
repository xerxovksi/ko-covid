namespace KO.Covid.Infrastructure.Cosmos
{
    using Microsoft.Azure.Cosmos;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.Azure.Cosmos.Linq;
    using System.Net;
    using KO.Covid.Domain.Exceptions;
    using KO.Covid.Application.Exceptions;

    public class CosmosDbClient : ICosmosDbClient
    {
        private readonly string databaseId = null;
        private readonly string containerId = null;
        private readonly CosmosClient cosmosClient = null;
        private Container container = null;

        public CosmosDbClient(
            string databaseName,
            CosmosClient cosmosClient,
            string containerId)
        {
            this.databaseId = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            this.containerId = containerId ?? throw new ArgumentNullException(nameof(containerId));
            this.cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));

            this.Initialize();
        }

        private void Initialize()
        {
            this.container = this.cosmosClient.GetContainer(this.databaseId, this.containerId);
        }

        public async Task<ItemResponse<T>> CreateItemAsync<T>(
            T item,
            PartitionKey? partitionKey = null,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default) =>
            await this.container.CreateItemAsync(
                item,
                partitionKey,
                itemRequestOptions,
                cancellationToken);

        public async Task<ItemResponse<T>> DeleteItemAsync<T>(
            string id,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default)
        {
            ItemResponse<T> result;

            try
            {
                result = await this.container.DeleteItemAsync<T>(
                    id,
                    partitionKey,
                    itemRequestOptions,
                    cancellationToken);
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new EntityNotFoundException(
                        $"Item not found with Id: {id} in Partition: {partitionKey}.",
                        exception);
                }
                else if (exception.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new TooManyRequestsException("Too many requests.");
                }

                throw;
            }

            return result;
        }

        public async Task<IEnumerable<T>> QueryItemsAsync<T>(
            Expression<Func<T, bool>> predicate,
            bool allowSynchronousQueryExecution = false,
            CancellationToken cancellationToken = default)
        {
            List<T> result = new List<T>();
            var feedIterator = this.container.GetItemLinqQueryable<T>(
                allowSynchronousQueryExecution)
                .Where(predicate)
                .ToFeedIterator();

            while (feedIterator.HasMoreResults)
            {
                foreach (var item in await feedIterator.ReadNextAsync(cancellationToken))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public async Task<long> GetItemsCountAsync<T>(
            Expression<Func<T, bool>> predicate,
            bool allowSynchronousQueryExecution = false,
            CancellationToken cancellationToken = default) =>
            await this.container.GetItemLinqQueryable<T>(
                allowSynchronousQueryExecution)
                .Where(predicate)
                .CountAsync();

        public async Task<IEnumerable<T>> QueryItemsAsync<T>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderByPredicate,
            int pageNumber,
            int pageSize,
            bool allowSynchronousQueryExecution = false,
            CancellationToken cancellationToken = default)
        {
            var result = new List<T>();
            int resultsToSkip = (pageNumber - 1) * pageSize;
            var feedIterator = this.container.GetItemLinqQueryable<T>(
                allowSynchronousQueryExecution,
                null)
                .Where(predicate)
                .OrderByDescending(orderByPredicate)
                .Skip(resultsToSkip)
                .Take(pageSize)
                .ToFeedIterator();

            while (feedIterator.HasMoreResults)
            {
                foreach (var item in await feedIterator.ReadNextAsync(cancellationToken))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public async Task<ItemResponse<T>> ReadItemAsync<T>(
            string id,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default) =>
            await this.container.ReadItemAsync<T>(
                id,
                partitionKey,
                itemRequestOptions,
                cancellationToken);

        public async Task<ItemResponse<T>> ReplaceItemAsync<T>(
            string id,
            T item,
            PartitionKey? partitionKey = null,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default) =>
            await this.container.ReplaceItemAsync(
                item,
                id,
                partitionKey,
                itemRequestOptions,
                cancellationToken);

        public async Task<ItemResponse<T>> UpsertItemAsync<T>(
            T item,
            PartitionKey? partitionKey = null,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default) =>
            await this.container.UpsertItemAsync(
                item,
                partitionKey,
                itemRequestOptions,
                cancellationToken);
    }
}
