using System;

namespace Uchu.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PacketHandlerAttribute : Attribute
    {
        public RemoteConnectionType? RemoteConnectionType { internal get; set; } = null;
        public uint? PacketId { internal get; set; } = null;
    }
}