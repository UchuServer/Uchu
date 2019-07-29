using System;

namespace Uchu.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PacketHandlerAttribute : Attribute
    {
        public RemoteConnectionType? RemoteConnectionType { get; set; } = null;
        
        public uint? PacketId { get; set; } = null;
        
        public bool RunTask { get; set; } = false;
    }
}