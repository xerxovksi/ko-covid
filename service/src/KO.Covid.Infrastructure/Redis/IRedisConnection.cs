namespace KO.Covid.Infrastructure.Redis
{
    using StackExchange.Redis;
    using System.Threading.Tasks;

    public interface IRedisConnection
    {
        ConnectionMultiplexer Connection { get; }

        IServer Server { get; }

        IDatabase Cache { get; }

        Task InitializeAsync();

        Task InitializeConnectionAsync();
    }
}
