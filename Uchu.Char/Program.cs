using System;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.Char
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var server = new Server(2002);

            await server.Resources.ReadTextAsync("Names/first.txt");
            
            server.Start();
        }
    }
}