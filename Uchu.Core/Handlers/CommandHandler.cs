using System.Reflection;

namespace Uchu.Core
{
    public class CommandHandler
    {
        public HandlerGroup Group { get; set; }
        
        public MethodInfo Info { get; set; }

        public GameMasterLevel GameMasterLevel { get; set; } = GameMasterLevel.Console;
        
        public string Signature { get; set; }
        
        public string Help { get; set; }
        
        public bool ConsoleCommand { get; set; }
    }
}