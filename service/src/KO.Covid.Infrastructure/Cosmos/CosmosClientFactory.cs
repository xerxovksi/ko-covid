namespace KO.Covid.Infrastructure.Cosmos
{
    using Microsoft.Azure.Cosmos;
    using System;
    using System.Collections.Generic;

    public class CosmosClientFactory : ICosmosClientFactory
    {
        private readonly string databaseId = null;
        private readonly List<string> containerIds = null;
        private readonly CosmosClient cosmosClient = null;

        public CosmosClientFactory(
            string databaseId,
            List<string> containerIds,
            CosmosClient cosmosClient)
        {
            this.databaseId = databaseId ?? throw new ArgumentNullException(nameof(databaseId));
            this.containerIds = containerIds ?? throw new ArgumentNullException(nameof(containerIds));
            this.cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
        }

        public ICosmosDbClient GetClient(string containerId)
        {
            if (!this.containerIds.Contains(containerId))
            {
                throw new ArgumentException($"Unable to find container: {containerId}");
            }

            return new CosmosDbClient(this.databaseId, this.cosmosClient, containerId);
        }

        public void EnsureDbSetupAsync()
        {
            var database = this.cosmosClient.GetDatabase(this.databaseId);

            foreach (var container in this.containerIds)
            {
                database.GetContainer(container);
            }
        }
    }
}
