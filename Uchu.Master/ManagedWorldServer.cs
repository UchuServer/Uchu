using System;
using Uchu.Core;

namespace Uchu.Master
{
    public class ManagedWorldServer : ManagedServer
    {
        public readonly ZoneId ZoneId;

        public readonly uint CloneId;

        public readonly ushort InstanceId;
        
        public int EmptyTime { get; set; }
        
        public ManagedWorldServer(Guid id, string location, string dotnet, ZoneId zoneId, uint cloneId, ushort instanceId) : base(id, location, dotnet)
        {
            ZoneId = zoneId;
            CloneId = cloneId;
            InstanceId = instanceId;
        }
    }
}