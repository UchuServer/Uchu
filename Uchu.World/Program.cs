using Uchu.Core;

namespace Uchu.World
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var server = new WorldServer(2003);

            server.Start();
        }
    }
}