using System;

namespace Uchu.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PacketHandlerAttribute : Attribute
    {
        public RemoteConnectionType? RemoteConnectionType { get; set; } = null;
        
        public uint? PacketId { get; set; } = null;
        
        [Obsolete("Handlers are non-blocking by default")]
        public bool RunTask { get; set; } = false;
    }
}