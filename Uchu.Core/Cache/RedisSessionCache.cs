using System;
using System.Net;
using System.Security.Cryptography;
using ServiceStack.Caching;
using ServiceStack.Redis;

namespace Uchu.Core
{
    public class RedisSessionCache : ISessionCache
    {
        private readonly RedisManagerPool _manager;
        private readonly ICacheClient _client;
        private readonly RNGCryptoServiceProvider _rng;

        public RedisSessionCache()
        {
            _manager = new RedisManagerPool();
            _client = _manager.GetCacheClient();
            _rng = new RNGCryptoServiceProvider();
        }

        public string CreateSession(IPEndPoint endpoint, long userId)
        {
            var key = _generateKey();

            _client.Set(endpoint.ToString(), new Session
            {
                Key = key,
                UserId = userId
            }, TimeSpan.FromDays(1));

            return key;
        }

        public void SetCharacter(IPEndPoint endpoint, long characterId)
        {
            var session = GetSession(endpoint);

            session.CharacterId = characterId;

            _client.Set(endpoint.ToString(), session, TimeSpan.FromDays(1));
        }

        public Session GetSession(IPEndPoint endpoint)
            => _client.Get<Session>(endpoint.ToString());

        public void DeleteSession(IPEndPoint endpoint)
            => _client.Remove(endpoint.ToString());

        private string _generateKey(int length = 24)
        {
            var bytes = new byte[length];

            _rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes).TrimEnd('=');
        }
    }
}