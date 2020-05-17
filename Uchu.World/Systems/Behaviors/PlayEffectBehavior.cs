using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class PlayEffectBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.PlayEffect;
        
        public int EffectId { get; set; }
        
        public override async Task BuildAsync()
        {
            var effectId = await GetParameter("effectID");
            
            if (effectId?.Value == null) return;

            EffectId = (int) effectId.Value;
        }
    }
}