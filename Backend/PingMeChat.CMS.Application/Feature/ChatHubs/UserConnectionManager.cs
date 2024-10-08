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
    }

    public class RedisConnectionManager : IRedisConnectionManager
    {
        // Sử dụng redis thay vì Dictionary
        // private readonly Dictionary<string, HashSet<string>> _userConnectionMap = new Dictionary<string, HashSet<string>>();
        // private readonly object _lock = new object();

        private readonly IConnectionMultiplexer _redis;
        private const string KeyPrefix = "user:connections:";
        public RedisConnectionManager(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        // public void AddConnection(string userId, string connectionId)
        // {
        //     lock (_lock)
        //     {
        //         if (!_userConnectionMap.ContainsKey(userId))
        //         {
        //             _userConnectionMap[userId] = new HashSet<string>();
        //         }
        //         _userConnectionMap[userId].Add(connectionId);
        //     }
        // }
        public async Task AddConnectionAsync(string userId, string connectionId)
        {
            var db = _redis.GetDatabase();
            await db.SetAddAsync($"{KeyPrefix}{userId}", connectionId);
        }

        // public void RemoveConnection(string userId, string connectionId)
        // {
        //     lock (_lock)
        //     {
        //         if (_userConnectionMap.ContainsKey(userId))
        //         {
        //             _userConnectionMap[userId].Remove(connectionId);
        //             if (_userConnectionMap[userId].Count == 0)
        //             {
        //                 _userConnectionMap.Remove(userId);
        //             }
        //         }
        //     }
        // }
        public async Task RemoveConnectionAsync(string userId, string connectionId)
        {
            var db = _redis.GetDatabase();
            await db.SetRemoveAsync($"{KeyPrefix}{userId}", connectionId);
        }
        // public List<string> GetConnections(string userId)
        // {
        //     lock (_lock)
        //     {
        //         if (_userConnectionMap.ContainsKey(userId))
        //         {
        //             return _userConnectionMap[userId].ToList();
        //         }
        //         return new List<string>();
        //     }
        // }
        public async Task<List<string>> GetConnectionsAsync(string userId)
        {
            var db = _redis.GetDatabase();
            var connections = await db.SetMembersAsync($"{KeyPrefix}{userId}");
            return connections.Select(c => c.ToString()).ToList();
        }
    }
}
