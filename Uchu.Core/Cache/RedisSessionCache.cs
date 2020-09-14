using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Uchu.Core
{
    /// <summary>
    /// A connection to a Redis cache server
    /// </summary>
    public sealed class RedisSessionCache : ISessionCache, IDisposable
    {
        private readonly IDatabase _client;
        private readonly RNGCryptoServiceProvider _rng;
        private readonly Dictionary<string, string> _keys = new Dictionary<string, string>();

        /// <summary>
        /// Creates a Redis cache connection based on the global config file
        /// </summary>
        /// <param name="config">The Redis cache configuration</param>
        public RedisSessionCache(CacheConfig config)
        {
            var manager = ConnectionMultiplexer.Connect($"{config.Host}:{config.Port}");
            _client = manager.GetDatabase();
                
            _rng = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Creates an active user session
        /// </summary>
        /// <param name="userId">The user ID to create a session for</param>
        /// <returns>The session key</returns>
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

        /// <summary>
        /// Creates an active user sessions using a specific key
        /// </summary>
        /// <param name="userId">The user ID to create a session for</param>
        /// <param name="key">The key that this session should be stored under</param>
        public void CreateSession(long userId, string key)
        {
            _client.StringSet(key, new Session
            {
                Key = key,
                UserId = userId
            }.ToBytes(), TimeSpan.FromDays(1));
        }

        /// <summary>
        /// Sets the character of a session
        /// </summary>
        /// <param name="endpoint">The endpoint a user is connecting from</param>
        /// <param name="characterId">The character to link to the connection</param>
        public void SetCharacter(IPEndPoint endpoint, long characterId)
        {
            var session = GetSession(endpoint);

            session.CharacterId = characterId;

            _client.StringSet(_keys[endpoint.ToString()], session.ToBytes(), TimeSpan.FromDays(1));
        }

        /// <summary>
        /// Sets the zone of a session
        /// </summary>
        /// <param name="endpoint">The endpoint a user is connecting from</param>
        /// <param name="zone">The zone to link to the connection</param>
        public void SetZone(IPEndPoint endpoint, ZoneId zone)
        {
            var session = GetSession(endpoint);

            session.ZoneId = (ushort) zone;

            _client.StringSet(_keys[endpoint.ToString()], session.ToBytes(), TimeSpan.FromDays(1));
        }

        /// <summary>
        /// Gets an active session based on a user connection
        /// </summary>
        /// <param name="endpoint">The endpoint a user is connecting from</param>
        /// <returns>If available, the session that belongs to the endpoint</returns>
        public Session GetSession(IPEndPoint endpoint)
        {
            var task = GetSessionAsync(endpoint);

            task.Wait();

            return GetSessionAsync(endpoint).Result;
        }

        /// <summary>
        /// Gets an active session based on a user connection
        /// </summary>
        /// <param name="endpoint">The endpoint a user is connecting from</param>
        /// <returns>If available, the session that belongs to the endpoint</returns>
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
        
        /// <summary>
        /// Checks if the given key exists in the cache
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Whether the key exists or not</returns>
        public bool IsKey(string key)
        {
            return _client.KeyExists(key);
        }

        /// <summary>
        /// Adds a key to the cache for a connection
        /// </summary>
        /// <param name="endPoint">The user connection to create an entry in the cache for</param>
        /// <param name="key">The key to register for the connection</param>
        public void RegisterKey(IPEndPoint endPoint, string key)
        {
            _keys.Add(endPoint.ToString(), key);
        }

        /// <summary>
        /// Removes a session from the cache
        /// </summary>
        /// <param name="endpoint">The user connection to remove from the cache</param>
        public void DeleteSession(IPEndPoint endpoint)
            => _client.KeyDelete(endpoint.ToString());

        /// <summary>
        /// Generates a random key
        /// </summary>
        /// <param name="length">The length of the key</param>
        /// <returns>A random key of the provided length</returns>
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