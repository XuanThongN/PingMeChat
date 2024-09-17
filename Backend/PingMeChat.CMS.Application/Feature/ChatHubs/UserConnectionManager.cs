using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.ChatHubs
{
    public interface IUserConnectionManager
    {
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string userId, string connectionId);
        List<string> GetConnections(string userId);
    }

    public class UserConnectionManager : IUserConnectionManager
    {
        private readonly Dictionary<string, HashSet<string>> _userConnectionMap = new Dictionary<string, HashSet<string>>();
        private readonly object _lock = new object();

        public void AddConnection(string userId, string connectionId)
        {
            lock (_lock)
            {
                if (!_userConnectionMap.ContainsKey(userId))
                {
                    _userConnectionMap[userId] = new HashSet<string>();
                }
                _userConnectionMap[userId].Add(connectionId);
            }
        }

        public void RemoveConnection(string userId, string connectionId)
        {
            lock (_lock)
            {
                if (_userConnectionMap.ContainsKey(userId))
                {
                    _userConnectionMap[userId].Remove(connectionId);
                    if (_userConnectionMap[userId].Count == 0)
                    {
                        _userConnectionMap.Remove(userId);
                    }
                }
            }
        }

        public List<string> GetConnections(string userId)
        {
            lock (_lock)
            {
                if (_userConnectionMap.ContainsKey(userId))
                {
                    return _userConnectionMap[userId].ToList();
                }
                return new List<string>();
            }
        }
    }
}
