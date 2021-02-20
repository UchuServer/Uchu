using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class PlayEffectBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.PlayEffect;

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            parameters.PlayFX(EffectId, target: parameters.Context.Associate);
        }
    }
}