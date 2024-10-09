using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace PingMeChat.CMS.Application.Feature.ChatHubs
{
    public interface IRedisConnectionManager
    {
        Task AddConnectionAsync(string userId, string connectionId);
        Task RemoveConnectionAsync(string userId, string connectionId);
        Task<List<string>> GetConnectionsAsync(string userId);
        Task<Dictionary<string, List<string>>> GetBulkConnectionsAsync(IEnumerable<string> userIds);
    }

    public class RedisConnectionManager : IRedisConnectionManager
    {
        private readonly IConnectionMultiplexer _redis; // Connection to Redis
        private readonly IDatabase _db; // Database Redis
        private const string KeyPrefix = "user:connections:"; // Key prefix cho Redis
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(1); // Thời gian tồn tại của cache
        public RedisConnectionManager(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
        }

        public async Task AddConnectionAsync(string userId, string connectionId)
        {
            await _db.SetAddAsync($"{KeyPrefix}{userId}", connectionId);
            await _db.KeyExpireAsync($"{KeyPrefix}{userId}", _cacheExpiry);
        }
        public async Task RemoveConnectionAsync(string userId, string connectionId)
        {
            await _db.SetRemoveAsync($"{KeyPrefix}{userId}", connectionId);
        }

        public async Task<List<string>> GetConnectionsAsync(string userId)
        {
            var connections = await _db.SetMembersAsync($"{KeyPrefix}{userId}");
            return connections.Select(c => c.ToString()).ToList();
        }
        public async Task<Dictionary<string, List<string>>> GetBulkConnectionsAsync(IEnumerable<string> userIds)
        {
            var batch = _db.CreateBatch();
            var tasks = userIds.Select(userId => batch.SetMembersAsync($"{KeyPrefix}{userId}")).ToList();
            batch.Execute();

            var results = await Task.WhenAll(tasks);
            return userIds.Zip(results, (userId, connections) =>
                new { UserId = userId, Connections = connections.Select(c => c.ToString()).ToList() })
                .ToDictionary(x => x.UserId, x => x.Connections);
        }

    }
}
