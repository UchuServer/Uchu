using System;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.Char
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server(2002);
            
            server.Start();
        }
    }
}