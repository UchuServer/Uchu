using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Uchu.Core.Resources;

namespace Uchu.Core
{
    public sealed class DatabaseCache : ISessionCache, IDisposable
    {
        private readonly RNGCryptoServiceProvider _rng;
        
        private readonly Dictionary<string, string> _keys = new Dictionary<string, string>();

        public DatabaseCache()
        {
            _rng = new RNGCryptoServiceProvider();
        }
        
        public string CreateSession(long userId)
        {
            using var ctx = new UchuContext();

            var key = GenerateKey();
            ctx.SessionCaches.Add(new SessionCache
            {
                Key = key,
                UserId = userId
            });

            ctx.SaveChanges();

            return key;
        }

        public void CreateSession(long userId, string key)
        {
            using var ctx = new UchuContext();

            ctx.SessionCaches.Add(new SessionCache
            {
                Key = key,
                UserId = userId
            });

            ctx.SaveChanges();
        }

        public void SetCharacter(IPEndPoint endpoint, long characterId)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint), 
                    ResourceStrings.DatabaseCache_SetCharacter_NullEndpointException);
            
            using var ctx = new UchuContext();

            var key = _keys[endpoint.ToString()];
            var session = ctx.SessionCaches.First(s => s.Key == key);
            session.CharacterId = characterId;

            ctx.SaveChanges();
        }

        public void SetZone(IPEndPoint endpoint, ZoneId zone)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint), 
                    ResourceStrings.DatabaseCache_SetZone_NullEndpointException);
            
            using var ctx = new UchuContext();
            var key = _keys[endpoint.ToString()];
            var session = ctx.SessionCaches.First(s => s.Key == key);
            session.ZoneId = zone;

            ctx.SaveChanges();
        }

        public Session GetSession(IPEndPoint endpoint)
        {
            var task = GetSessionAsync(endpoint);
            task.Wait();
            return task.Result;
        }

        [SuppressMessage("ReSharper", "CA2000")]
        public async Task<Session> GetSessionAsync(IPEndPoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint), 
                    ResourceStrings.DatabaseCache_GetSessionAsync_NullEndpointException);
            
            await using var ctx = new UchuContext();
            
            string key;
            var timeout = 1000;
            
            while (!_keys.TryGetValue(endpoint.ToString(), out key))
            {
                if (timeout <= 0)
                    throw new TimeoutException();
                
                await Task.Delay(50).ConfigureAwait(false);

                timeout -= 50;
            }

            var session = ctx.SessionCaches.First(s => s.Key == key);
            
            return new Session
            {
                Key = key,
                CharacterId = session.CharacterId,
                UserId = session.UserId,
                ZoneId = session.ZoneId
            };
        }

        public bool IsKey(string key)
        {
            using var ctx = new UchuContext();
            return ctx.SessionCaches.Any(c => c.Key == key);
        }

        public void RegisterKey(IPEndPoint endPoint, string key)
        {
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint),
                    ResourceStrings.DatabaseCache_RegisterKey_NullEndpointException);
            
            _keys.Add(endPoint.ToString(), key);
        }

        public void DeleteSession(IPEndPoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint), 
                    ResourceStrings.DatabaseCache_DeleteSession_NullEndpointException);
            
            using var ctx = new UchuContext();

            var key = _keys[endpoint.ToString()];
            var session = ctx.SessionCaches.First(s => s.Key == key);
            ctx.SessionCaches.Remove(session);

            ctx.SaveChanges();
        }

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