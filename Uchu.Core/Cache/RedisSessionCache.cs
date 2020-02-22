using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Uchu.Core
{
    public sealed class RedisSessionCache : ISessionCache, IDisposable
    {
        private readonly IDatabase _client;
        private readonly RNGCryptoServiceProvider _rng;
        private readonly Dictionary<string, string> _keys = new Dictionary<string, string>();

        public RedisSessionCache()
        {
            var manager = ConnectionMultiplexer.Connect("localhost");
            _client = manager.GetDatabase();
                
            _rng = new RNGCryptoServiceProvider();
        }

        public string CreateSession(long userId)
        {
            var key = GenerateKey();

            _client.StringSet(key, new Session
            {
                Key = key,
                UserId = userId
            }.ToBytes(), TimeSpan.FromDays(1));

            return key;
        }

        public void CreateSession(long userId, string key)
        {
            _client.StringSet(key, new Session
            {
                Key = key,
                UserId = userId
            }.ToBytes(), TimeSpan.FromDays(1));
        }

        public void SetCharacter(IPEndPoint endpoint, long characterId)
        {
            var session = GetSession(endpoint);

            session.CharacterId = characterId;

            _client.StringSet(_keys[endpoint.ToString()], session.ToBytes(), TimeSpan.FromDays(1));
        }

        public void SetZone(IPEndPoint endpoint, ZoneId zone)
        {
            var session = GetSession(endpoint);

            session.ZoneId = (ushort) zone;

            _client.StringSet(_keys[endpoint.ToString()], session.ToBytes(), TimeSpan.FromDays(1));
        }

        public Session GetSession(IPEndPoint endpoint)
        {
            var task = GetSessionAsync(endpoint);

            task.Wait();

            return task.Result;
        }

        public async Task<Session> GetSessionAsync(IPEndPoint endpoint)
        {
            string key;
            
            var timeout = 1000;
            
            while (!_keys.TryGetValue(endpoint.ToString(), out key))
            {
                if (timeout <= 0)
                    throw new TimeoutException();
                
                await Task.Delay(50).ConfigureAwait(false);

                timeout -= 50;
            }
            
            return Session.FromBytes(_client.StringGet(key));
        }
        
        public bool IsKey(string key)
        {
            return _client.KeyExists(key);
        }

        public void RegisterKey(IPEndPoint endPoint, string key)
        {
            _keys.Add(endPoint.ToString(), key);
        }

        public void DeleteSession(IPEndPoint endpoint)
            => _client.KeyDelete(endpoint.ToString());

        private string GenerateKey(int length = 24)
        {
            var bytes = new byte[length];

            _rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes).TrimEnd('=');
        }

        public void Dispose()
        {
            _rng.Dispose();
        }
    }
}