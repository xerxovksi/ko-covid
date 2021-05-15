namespace KO.Covid.Infrastructure.Cosmos
{
    using Microsoft.Azure.Cosmos;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using System.Threading;
    using KO.Covid.Domain.Entities;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application;
    using KO.Covid.Domain.Exceptions;
    using KO.Covid.Application.Exceptions;

    public abstract class CosmosDbRepository<T>
        : IRepository<T>, IContainerContext<T> where T : Entity, new()
    {
        private readonly ICosmosClientFactory cosmosClientFactory = null;

        protected CosmosDbRepository(ICosmosClientFactory cosmosClientFactory)
        {
            this.cosmosClientFactory = cosmosClientFactory;
        }

        public abstract string ContainerId { get; }

        public async Task<T> AddItemAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            try
            {
                entity.Id = string.IsNullOrWhiteSpace(entity.Id)
                    ? this.GenerateId(entity)
                    : entity.Id;

                var cosmosClient = this.cosmosClientFactory.GetClient(this.ContainerId);
                var response = await cosmosClient.CreateItemAsync<T>(
                    entity,
                    this.ResolvePartitionKey(entity),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(true);

                return response.Resource;
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new EntityAlreadyExistsException(
                        $"Item already exists in Partition: {this.ResolvePartitionKey(entity)}.",
                        exception);
                }
                else if (exception.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new TooManyRequestsException("Too many requests.");
                }

                throw;
            }
        }

        public async Task<bool> DeleteItemAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cosmosClient = this.cosmosClientFactory.GetClient(this.ContainerId);
                var response = await cosmosClient.DeleteItemAsync<T>(
                    entity.Id,
                    this.ResolvePartitionKey(entity),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(true);

                return response.StatusCode.IsSuccessStatusCode();
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new EntityNotFoundException(
                        $"Item not found with Id: {entity.Id} in Partition: {this.ResolvePartitionKey(entity)}.",
                        exception);
                }
                else if (exception.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new TooManyRequestsException("Too many requests.");
                }

                throw;
            }
        }

        public virtual string GenerateId(T entity) => Guid.NewGuid().ToString();

        public async Task<T> GetItemByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            T entity = default;
            var partitionKey = this.ResolvePartitionKey(entity);

            try
            {
                var cosmosClient = this.cosmosClientFactory.GetClient(this.ContainerId);
                var response = await cosmosClient.ReadItemAsync<T>(id, partitionKey, cancellationToken: cancellationToken).ConfigureAwait(true);

                return response.Resource;
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new EntityNotFoundException(
                        $"Item not found with Id: '{id}' in Partition: {partitionKey}.",
                        exception);
                }
                else if (exception.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new TooManyRequestsException("Too many requests.");
                }

                throw;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(
            Expression<Func<T, bool>> predicate,
            bool allowSynchronousQueryExecution = false,
            CancellationToken cancellationToken = default)
        {
            var cosmosClient = this.cosmosClientFactory.GetClient(this.ContainerId);
            return await cosmosClient.QueryItemsAsync(
                predicate,
                allowSynchronousQueryExecution,
                cancellationToken: cancellationToken)
                .ConfigureAwait(true);
        }

        public async Task<IEnumerable<T>> GetItemsAsync(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderByPredicate,
            int pageNumber,
            int pageSize,
            bool allowSynchronousQueryExecution = false,
            CancellationToken cancellationToken = default)
        {
            var cosmosClient = this.cosmosClientFactory.GetClient(this.ContainerId);
            return await cosmosClient.QueryItemsAsync<T>(
                predicate,
                orderByPredicate,
                pageNumber,
                pageSize,
                allowSynchronousQueryExecution,
                cancellationToken: cancellationToken)
                .ConfigureAwait(true);
        }

        public async Task<long> GetItemsCountAsync(
            Expression<Func<T, bool>> predicate,
            bool allowSynchronousQueryExecution = false,
            CancellationToken cancellationToken = default)
        {
            var cosmosClient = this.cosmosClientFactory.GetClient(this.ContainerId);
            return await cosmosClient.GetItemsCountAsync<T>(
                predicate,
                allowSynchronousQueryExecution,
                cancellationToken: cancellationToken)
                .ConfigureAwait(true);
        }

        public virtual PartitionKey ResolvePartitionKey(T entity) =>
            new PartitionKey(string.Empty);

        public async Task<T> UpdateItemAsync(T entity, CancellationToken cancellationToken = default)
        {
            var partitionKey = this.ResolvePartitionKey(entity);

            try
            {
                var cosmosClient = this.cosmosClientFactory.GetClient(this.ContainerId);
                var response = await cosmosClient.ReplaceItemAsync(
                    entity.Id,
                    entity,
                    partitionKey,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(true);

                return response.Resource;
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new EntityNotFoundException(
                        $"Item not found with Id: {entity.Id} in Partition: {partitionKey}.",
                        exception);
                }
                else if (exception.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new EntityAlreadyExistsException(
                        $"Item already exists in Partition: {partitionKey}.",
                        exception);
                }
                else if (exception.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new TooManyRequestsException("Too many requests.");
                }

                throw;
            }
        }

        public async Task<T> UpsertItemAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            var partitionKey = this.ResolvePartitionKey(entity);
            try
            {
                var cosmosClient = this.cosmosClientFactory.GetClient(this.ContainerId);
                var response = await cosmosClient.UpsertItemAsync(
                    entity,
                    partitionKey,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(true);

                return response.Resource;
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new EntityAlreadyExistsException(
                        $"Item already exists in Partition: {partitionKey}.",
                        exception);
                }
                else if (exception.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new TooManyRequestsException("Too many requests.");
                }

                throw;
            }
        }
    }
}
