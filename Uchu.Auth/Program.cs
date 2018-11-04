using System.Reflection;
using System.Threading;
using Uchu.Core;

namespace Uchu.Auth
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server(1001);

            server.RegisterAssembly(Assembly.GetEntryAssembly());

            server.Start();

            Thread.Sleep(-1);
        }
    }
}