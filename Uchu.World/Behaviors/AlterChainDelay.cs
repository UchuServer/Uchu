using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class AlterChainDelay : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AlterChainDelay;
        
        public override async Task Serialize(BitReader reader)
        {
            // TODO
            return;
        }
    }
}