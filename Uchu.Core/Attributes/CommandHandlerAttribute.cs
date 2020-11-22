using System;

namespace Uchu.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandHandlerAttribute : Attribute
    {
        public char Prefix { get; set; } = '/';
        
        public string Signature { get; set; }

        public string Help { get; set; }

        public int GameMasterLevel { get; set; } = 9;
    }
}