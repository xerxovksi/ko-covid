namespace KO.Covid.Application.Contracts
{
    using KO.Covid.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> where TEntity : Entity, new()
    {
        Task<TEntity> AddItemAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteItemAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> GetItemsAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool allowSynchronousQueryExecution = false,
            CancellationToken cancellationToken = default);

        Task<TEntity> UpdateItemAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        Task<TEntity> UpsertItemAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);
    }
}
