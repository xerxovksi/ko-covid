﻿namespace KO.Covid.Application.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICache<T>
    {
        Task<T> GetAsync(string key, Func<string, T> converter);

        Task<Dictionary<string, T>> GetAllAsync(string pattern, Func<string, T> converter);

        Task<bool> SetAsync(string key, TimeSpan timeToLive, Func<string> converter);

        Task<bool> RemoveAsync(string key);

        Task<bool> RemoveAllAsync(string pattern = default);
    }
}
