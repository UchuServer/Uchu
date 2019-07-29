using System.Reflection;

namespace Uchu.Core
{
    public class Handler
    {
        public HandlerGroup Group { get; set; }
        
        public IPacket Packet { get; set; }
        
        public MethodInfo Info { get; set; }
        
        public bool RunTask { get; set; }
    }
}