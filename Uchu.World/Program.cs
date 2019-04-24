using System.Reflection;
using Uchu.Core;

namespace Uchu.World
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server(ServerType.World);

            server.RegisterAssembly(Assembly.GetEntryAssembly());

            server.Start();
        }
    }
}