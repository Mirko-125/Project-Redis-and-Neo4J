using System.Drawing;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace Cache
{
    public class RedisCache {
        private static RedisCache _instance;
        private readonly IRedisClientsManager _redisClientManager;

        // Private constructor ensures controlled instantiation
        private RedisCache(IRedisClientsManager redisClientManager)
        {
            _redisClientManager = redisClientManager;
        }

        // Method to initialize the singleton instance
        public static RedisCache Initialize(IRedisClientsManager redisClientManager)
        {
            if (_instance == null)
            {
                _instance = new RedisCache(redisClientManager);
            }

            return _instance;
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

        public async Task<bool> SetDataAsync < T > (string key, T value, int expirationtime)
        {
            var redisClient = await _redisClientManager.GetClientAsync();
            var stringData = JsonSerializer.SerializeToString(value);
            TimeSpan time = TimeSpan.FromSeconds(expirationtime);
            var success = await redisClient.SetAsync(key, stringData, time);
            return success;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            var redisClient = await _redisClientManager.GetClientAsync();
            var success = await redisClient.RemoveAsync(key);
            return success;
        }
    }
}