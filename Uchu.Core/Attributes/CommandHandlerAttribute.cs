using System;

namespace Uchu.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandHandlerAttribute : Attribute
    {
        public char Prefix { get; set; } = '/';
        
        public string Signature { get; set; }

        public string Help { get; set; }

        public GameMasterLevel GameMasterLevel { get; set; } = GameMasterLevel.Console;
    }
}