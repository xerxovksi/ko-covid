namespace KO.Covid.Infrastructure.Cosmos
{
    using Microsoft.Azure.Cosmos;
    using KO.Covid.Domain.Entities;

    public interface IContainerContext<in T> where T : Entity
    {
        string ContainerId { get; }

        string GenerateId(T entity);

        PartitionKey ResolvePartitionKey(T entity);
    }
}
