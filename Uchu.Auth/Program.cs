using Uchu.Core;

namespace Uchu.Auth
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server(21836);
            
            server.Start();
        }
    }
}
