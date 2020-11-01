using System;
using System.Reflection;

namespace Uchu.Core
{
    /// <summary>
    /// Handler class that represents a method packet handler in a handler group
    /// </summary>
    public class Handler
    {
        public Handler(HandlerGroup group, MethodInfo info, Type packetType)
        {
            Group = group;
            Info = info;
            PacketType = packetType;
        }
        
        /// <summary>
        /// The handler group of this packet handler
        /// </summary>
        public HandlerGroup Group { get; set; }
        
        /// <summary>
        /// The type of the packet this handler handles
        /// </summary>
        public Type PacketType { get; set; }

        /// <summary>
        /// Methodinfo regarding the handler this handler represents
        /// </summary>
        public MethodInfo Info { get; set; }

        /// <summary>
        /// Creates a new packet from the type this handler represents
        /// </summary>
        /// <returns>An empty packet of type <c>Type</c></returns>
        public IPacket NewPacket()
        {
            return (IPacket) Activator.CreateInstance(PacketType);
        }
    }
}