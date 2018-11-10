using System.Reflection;

namespace Uchu.Core
{
    public class Handler
    {
        public HandlerGroupBase Group { get; set; }
        public IPacket Packet { get; set; }
        public MethodInfo Method { get; set; }
    }
}