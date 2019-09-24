using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.Auth
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var server = new Server(ServerType.Authentication);
            
            await server.StartAsync();
        }
    }
}
