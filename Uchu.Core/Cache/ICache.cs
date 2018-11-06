using System.Net;

namespace Uchu.Core
{
    public interface ICache
    {
        string CreateSession(IPEndPoint endpoint, long userId);

        void SetCharacter(IPEndPoint endpoint, long characterId);

        Session GetSession(IPEndPoint endpoint);

        void DeleteSession(IPEndPoint endpoint);
    }
}