using System.Net;

namespace Uchu.Core
{
    public interface ICache
    {
        string CreateSession(IPEndPoint endpoint, long userId);

        Session GetSession(IPEndPoint endpoint);

        void DeleteSession(IPEndPoint endpoint);
    }
}