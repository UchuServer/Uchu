using System.Reflection;
using Uchu.Core;

namespace Uchu.World
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server(2003);

            server.RegisterAssembly(Assembly.GetEntryAssembly());

            server.Start();
        }
    }
}