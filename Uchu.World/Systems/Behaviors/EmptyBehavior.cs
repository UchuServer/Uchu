using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class EmptyBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Empty;
    }
}