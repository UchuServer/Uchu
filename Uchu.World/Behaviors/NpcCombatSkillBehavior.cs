using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class NpcCombatSkillBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.NPCCombatSkill;
        
        public BehaviorBase Behavior { get; set; }
        
        public float MinRange { get; set; }
        
        public float MaxRange { get; set; }
        
        public override async Task BuildAsync()
        {
            Behavior = await GetBehavior("Behavior 1");

            MinRange = await GetParameter<float>("min range");

            MaxRange = await GetParameter<float>("max range");
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            context.MinRange = MaxRange;
            context.MaxRange = MaxRange;
            
            if (!context.Associate.TryGetComponent<BaseCombatAiComponent>(out var baseCombatAiComponent)) return;

            await Behavior.CalculateAsync(context, branchContext);
        }
    }
}