﻿using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Event_Management.Extensions
{
    public static class RedisExtentions
    {
        public static async Task SetValue<T>(this IDistributedCache cache, string key, T data,
            TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(1);
            options.SlidingExpiration = unusedExpireTime;

            var jsonData = JsonSerializer.Serialize(data);

            await cache.SetStringAsync(key, jsonData);
        }

        public static async Task<T> GetValue<T>(this IDistributedCache cache, string key)
        {
            var jsonData = await cache.GetStringAsync(key);
            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
