using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class PlayEffectBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.PlayEffect;
        private int InternalEffectId { get; set; }
        
        public override void Build()
        {
            var effectId = GetParameter("effectID");
            if (effectId?.Value == null) return;
            InternalEffectId = (int) effectId.Value;
        }
    }
}