using System;

namespace Uchu.Core
{
    public class WorldServerRequest
    {
        public Guid Id { get; set; }
        
        public ZoneId ZoneId { get; set; }
        
        public WorldServerRequestState State { get; set; }
        
        public Guid SpecificationId { get; set; }
    }
}