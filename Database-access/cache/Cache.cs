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
        
        private RedisCache(IRedisClientsManager redisClientManager)
        {
            _redisClientManager = redisClientManager;
        }

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
            try
            {
                var keyExists = await redisClient.ContainsKeyAsync(key);
                return keyExists;
            }
            finally
            {
                await redisClient.DisposeAsync();
            }
        }

        public async Task<T> GetDataAsync < T > (string key) 
        {
            var redisClient = await _redisClientManager.GetClientAsync();
            try
            {
                var keyExists = await redisClient.ContainsKeyAsync(key);
                if (keyExists)
                {
                    var res = await redisClient.GetAsync<string>(key);
                    return JsonSerializer.DeserializeFromString<T>(res);
                }
                return default;
            }
            finally
            {
                await redisClient.DisposeAsync();
            }
        }

        public async Task<bool> SetDataAsync < T > (string key, T value, int expirationtime)
        {
            var redisClient = await _redisClientManager.GetClientAsync();
            try
            {
                 var stringData = JsonSerializer.SerializeToString(value);
                TimeSpan time = TimeSpan.FromSeconds(expirationtime);
                var success = await redisClient.SetAsync(key, stringData, time);
                return success;
            }
            finally
            {
                await redisClient.DisposeAsync();
            }
           
        }

        public async Task<bool> DeleteAsync(string key)
        {
            var redisClient = await _redisClientManager.GetClientAsync();
            try 
            {
                var success = await redisClient.RemoveAsync(key);
                return success;
            }
            finally
            {
                await redisClient.DisposeAsync();
            }

        }
    }
}