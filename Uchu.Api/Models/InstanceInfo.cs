using System;
using System.Collections.Generic;

namespace Uchu.Api.Models
{
    public class InstanceInfo
    {
        public int MasterApi { get; set; }
        
        public Guid Id { get; set; }
        
        public int Port { get; set; }
        
        public int ApiPort { get; set; }
        
        public int Type { get; set; }
        
        public List<int> Zones { get; set; }
    }
}