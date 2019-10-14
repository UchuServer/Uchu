using System;

namespace Uchu.Core
{
    public class ServerSpecification
    {
        public Guid Id { get; set; }
        
        public ServerType ServerType { get; set; }
        
        public int Port { get; set; }
        
        public uint MaxUserCount { get; set; }
        
        public uint ActiveUserCount { get; set; }
        
        public ZoneId ZoneId { get; set; }
        
        public uint ZoneCloneId { get; set; }
        
        public ushort ZoneInstanceId { get; set; }
    }
}