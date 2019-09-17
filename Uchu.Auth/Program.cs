using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.Auth
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var server = new Server(21836);
            
            await server.Start();
        }
    }
}
