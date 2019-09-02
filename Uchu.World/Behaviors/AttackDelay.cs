using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class AttackDelay : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AttackDelay;
        
        public override async Task Serialize(BitReader reader)
        {
            // TODO
            return;
        }
    }
}