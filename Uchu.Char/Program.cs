using System.Reflection;
using Uchu.Core;

namespace Uchu.Char
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server(ServerType.Character);

            server.RegisterAssembly(Assembly.GetEntryAssembly());

            server.Start();
        }
    }
}