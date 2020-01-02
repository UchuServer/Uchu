using System.Collections.Generic;
using InfectedRose.Luz;
using InfectedRose.Lvl;
using InfectedRose.Triggers;

namespace Uchu.World.Client
{
    public class ZoneInfo
    {
        public LuzFile LuzFile { get; set; }
        
        public List<Trigger> Triggers { get; set; }
        
        public List<LvlFile> LvlFiles { get; set; }
    }
}