using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace Cache
{
    public class RedisCache {
        private readonly IRedisClientsManager _redisClientManager;

        public RedisCache(IRedisClientsManager redisClientManager)
        {
            _redisClientManager = redisClientManager;
  
        }

        public async Task<bool> CheckKeyAsync(string key)
        {
            var redisClient = await _redisClientManager.GetClientAsync();
            var keyExists = await redisClient.ContainsKeyAsync(key);
            return keyExists;
        }

        public async Task<T> GetDataAsync < T > (string key) 
        {
            var redisClient = await _redisClientManager.GetClientAsync();
            var keyExists = await redisClient.ContainsKeyAsync(key);
            if (keyExists)
            {
                var json = await redisClient.GetAsync<string>(key);
                return JsonSerializer.DeserializeFromString<T>(json);
            }
            return default;
        }

        public async Task<bool> SetDataAsync < T > (string key, T value)
        {
            var redisClient = await _redisClientManager.GetClientAsync();
            var stringData = JsonSerializer.SerializeToString(value);
            var success = await redisClient.SetAsync(key, stringData);
            return success;
        }
    }
}