using System.Reflection;
using Uchu.Core;

namespace Uchu.Char
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server(2002);

            server.RegisterAssembly(Assembly.GetEntryAssembly());

            server.Start();
        }
    }
}