using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class Stun : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Stun;
        
        public override async Task Serialize(BitReader reader)
        {
            // TODO
            return;
        }
    }
}