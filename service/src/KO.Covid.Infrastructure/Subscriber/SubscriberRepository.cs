namespace KO.Covid.Infrastructure.Subscriber
{
    using KO.Covid.Domain.Entities;
    using KO.Covid.Infrastructure.Cosmos;
    using Microsoft.Azure.Cosmos;

    public class SubscriberRepository<T> :
        CosmosDbRepository<T> where T : Subscriber, new()
    {
        public override string ContainerId { get; }

        public SubscriberRepository(
            ICosmosClientFactory cosmosClientFactory,
            SubscriberContainer container)
            : base(cosmosClientFactory) =>
            this.ContainerId = container.Id;

        public override PartitionKey ResolvePartitionKey(T entity) =>
            new PartitionKey(entity.Mobile);
    }
}
