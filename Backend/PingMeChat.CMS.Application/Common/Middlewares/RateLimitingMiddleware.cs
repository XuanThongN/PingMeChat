using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Common.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly int _maxRequests; // Số lượng lệnh tối đa trong khoảng thời gian
        private readonly TimeSpan _interval; // Khoảng thời gian 

        // Hàm khởi tạo middleware
        // maxRequests: Số lượng lệnh tối đa trong khoảng thời gian
        // intervalInSeconds: Khoảng thời gian tính bằng giây
        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache, int maxRequests = 2000, int intervalInSeconds = 60)
        {
            _next = next;
            _cache = cache;
            _maxRequests = maxRequests;
            _interval = TimeSpan.FromSeconds(intervalInSeconds);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var key = GenerateClientKey(context);
            var clientStatistics = GetClientStatistics(key);

            if (clientStatistics.Count >= _maxRequests)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsync("Giới hạn lượt truy cập. Vui lòng thử lại sau.");
                return;
            }

            clientStatistics.Count++;
            _cache.Set(key, clientStatistics, clientStatistics.ExpirationTime);

            await _next(context);
        }

        private string GenerateClientKey(HttpContext context)
        {
            return $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";
        }

        private ClientStatistics GetClientStatistics(string key)
        {
            if (!_cache.TryGetValue(key, out ClientStatistics clientStatistics))
            {
                clientStatistics = new ClientStatistics
                {
                    Count = 0,
                    ExpirationTime = DateTime.UtcNow.Add(_interval)
                };
            }
            else if (clientStatistics.ExpirationTime <= DateTime.UtcNow)
            {
                clientStatistics = new ClientStatistics
                {
                    Count = 0,
                    ExpirationTime = DateTime.UtcNow.Add(_interval)
                };
            }

            return clientStatistics;
        }

        private class ClientStatistics
        {
            public int Count { get; set; }
            public DateTime ExpirationTime { get; set; }
        }
    }
}
