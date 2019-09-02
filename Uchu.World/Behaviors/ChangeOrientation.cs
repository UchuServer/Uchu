using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class ChangeOrientation : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ChangeOrientation;
        
        public override async Task Serialize(BitReader reader)
        {
            // TODO
            return;
        }
    }
}