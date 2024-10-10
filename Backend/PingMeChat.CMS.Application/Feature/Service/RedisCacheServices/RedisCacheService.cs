using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PingMeChat.CMS.Application.Feature.Services.RedisCacheServices
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;


        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger, IOptions<JsonOptions> jsonOptions)
        {
            _cache = cache;
            _logger = logger;
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;

        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            try
            {
                _logger.LogInformation($"GetOrCreateAsync: {key}");

                var cachedResult = await _cache.GetStringAsync(key);

                if (cachedResult != null)
                {
                    var deserializedResult = JsonSerializer.Deserialize<T>(cachedResult, _jsonOptions);
                    if (deserializedResult == null)
                    {
                        throw new InvalidOperationException("Deserialization returned null.");
                    }
                    return deserializedResult;
                }

                var result = await factory();

                var options = new DistributedCacheEntryOptions();
                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration;
                }

                await _cache.SetStringAsync(key, JsonSerializer.Serialize(result, _jsonOptions), options);

                return result;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error in RedisCacheService" + ex.Message);
                throw;
            }
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}