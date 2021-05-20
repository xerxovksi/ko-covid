namespace KO.Covid.Infrastructure.Redis
{
    using KO.Covid.Application;
    using StackExchange.Redis;
    using System.Linq;
    using System.Threading.Tasks;

    public class RedisConnection : IRedisConnection
    {
        private const int MaximumRetryAttempts = 5;

        private readonly ExecutionPolicy policy = null;
        private readonly string connectionString = null;

        public ConnectionMultiplexer Connection { get; private set; }

        public IServer Server { get; private set; }

        public IDatabase Cache { get; private set; }

        public RedisConnection(string connectionString)
        {
            this.policy = new ExecutionPolicy();
            this.connectionString = connectionString;
        }

        public async Task InitializeAsync()
        {
            if (this.Connection?.IsConnected ?? false)
            {
                return;
            }

            await this.InitializeConnectionAsync();

            this.Server = this.Connection.GetServer(this.Connection.GetEndPoints().First());
            this.Cache = this.Connection.GetDatabase();
        }

        public async Task InitializeConnectionAsync()
        {
            if (this.Connection == null)
            {
                this.Connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
                return;
            }

            if (this.Connection.IsConnecting)
            {
                this.policy.WithRetry(
                    operation: () => this.Connection.IsConnected,
                    isSuccessful: isConnected => isConnected,
                    maximumRetryCount: MaximumRetryAttempts,
                    shouldThrowIfMaximumRetriesExceeded: true);

                return;
            }

            if (this.Connection.IsConnected)
            {
                return;
            }

            this.Connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
        }
    }
}
