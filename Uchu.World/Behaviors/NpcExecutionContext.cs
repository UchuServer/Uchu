using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class NpcExecutionContext : ExecutionContext
    {
        public float MinRange { get; set; }
        
        public float MaxRange { get; set; }
        
        public NpcExecutionContext(GameObject associate, BitWriter writer) : base(associate, default, writer)
        {
        }
    }
}