namespace KO.Covid.Infrastructure.Redis
{
    using KO.Covid.Application.Contracts;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class RedisCache<T> : ICache<T> where T : class
    {
        private readonly IRedisConnection redis = null;

        public RedisCache(IRedisConnection redis) =>
            this.redis = redis;

        public async Task<T> GetAsync(string key, Func<string, T> converter)
        {
            await this.redis.InitializeAsync();
            return converter(await this.redis.Cache.StringGetAsync(key));
        }

        public async Task<Dictionary<string, T>> GetAllAsync(string pattern, Func<string, T> converter)
        {
            await this.redis.InitializeAsync();

            var keys = this.GetAllKeys(this.redis.Server, pattern);
            var values = new Dictionary<string, T>();

            while (await keys.MoveNextAsync())
            {
                values.Add(
                    keys.Current,
                    converter(await this.redis.Cache.StringGetAsync(keys.Current)));
            }

            return values;
        }

        public async Task<bool> SetAsync(string key, TimeSpan expiry, Func<string> converter)
        {
            await this.redis.InitializeAsync();
            return await this.redis.Cache.StringSetAsync(key, converter(), expiry);
        }

        public async Task<bool> RemoveAsync(string key)
        {
            await this.redis.InitializeAsync();
            return await this.redis.Cache.KeyDeleteAsync(key);
        }

        public async Task<bool> RemoveAllAsync(string pattern)
        {
            await this.redis.InitializeAsync();

            var areAllKeysRemoved = true;
            var keys = this.GetAllKeys(this.redis.Server, pattern);

            while (await keys.MoveNextAsync())
            {
                areAllKeysRemoved &= await this.redis.Cache.KeyDeleteAsync(keys.Current);
            }

            return areAllKeysRemoved;
        }

        private IAsyncEnumerator<RedisKey> GetAllKeys(IServer server, string pattern) =>
            server.KeysAsync(pattern: pattern).GetAsyncEnumerator();
    }
}
