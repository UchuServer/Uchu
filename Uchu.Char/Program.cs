using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.Char
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var server = new Server(ServerType.Character);
            
            await server.StartAsync();
        }
    }
}