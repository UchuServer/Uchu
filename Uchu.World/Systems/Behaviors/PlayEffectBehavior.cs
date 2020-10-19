using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class PlayEffectBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.PlayEffect;
        private int InternalEffectId { get; set; }
        
        public override async Task BuildAsync()
        {
            var effectId = await GetParameter("effectID");
            if (effectId?.Value == null) return;
            InternalEffectId = (int) effectId.Value;
        }
    }
}