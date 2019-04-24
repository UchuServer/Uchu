using System.Reflection;
using Uchu.Core;

namespace Uchu.Auth
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server(ServerType.Authentication); // use 21836 instead of 1001 since we're using the TCP/UDP protocol now

            server.RegisterAssembly(Assembly.GetEntryAssembly());

            server.Start();
        }
    }
}