using System.Text.Json;
using System.Text.Json.Serialization;
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
        // Hàm GetAsync
        Task<T> GetAsync<T>(string key);
        // Hàm SetAsync
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
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
            _jsonOptions = new JsonSerializerOptions(jsonOptions.Value.JsonSerializerOptions);
            _jsonOptions.Converters.Add(new UriConverter());

        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                _logger.LogInformation($"GetAsync: {key}");

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

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAsync: {ex.Message}");
                throw;
            }
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

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                Console.WriteLine($"SetAsync: {key}");

                var options = new DistributedCacheEntryOptions();
                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration;
                }

                var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
                await _cache.SetStringAsync(key, serializedValue, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetAsync: {ex.Message}");
                throw;
            }
        }
    }
}

public class UriConverter : JsonConverter<Uri>
{
    public override Uri Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var uriString = reader.GetString();
        return string.IsNullOrEmpty(uriString) ? null : new Uri(uriString);
    }

    public override void Write(Utf8JsonWriter writer, Uri value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }
}