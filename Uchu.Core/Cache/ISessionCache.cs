using System.Net;

namespace Uchu.Core
{
    public interface ISessionCache
    {
        string CreateSession(IPEndPoint endpoint, long userId);

        void SetCharacter(IPEndPoint endpoint, long characterId);

        void SetZone(IPEndPoint endpoint, ZoneId zone);

        Session GetSession(IPEndPoint endPoint);

        bool IsKey(string key);

        void RegisterKey(IPEndPoint endPoint, string key);

        void DeleteSession(IPEndPoint endpoint);
    }
}