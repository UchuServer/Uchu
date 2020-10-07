using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class SkillCastFailedBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.SkillCastFailed;
        private int InnerEffectId { get; set; }
        public override async Task BuildAsync()
        {
            InnerEffectId = await GetParameter<int>("effect_id");
        }
    }
}