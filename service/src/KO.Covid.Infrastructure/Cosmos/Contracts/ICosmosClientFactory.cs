namespace KO.Covid.Infrastructure.Cosmos
{
    public interface ICosmosClientFactory
    {
        ICosmosDbClient GetClient(string containerId);
    }
}
