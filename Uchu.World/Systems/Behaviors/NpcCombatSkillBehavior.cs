using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class NpcCombatSkillBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.NPCCombatSkill;

        private BehaviorBase Behavior { get; set; }
        private float MinRange { get; set; }
        private float MaxRange { get; set; }
        private float SkillTime { get; set; }
        
        public override async Task BuildAsync()
        {
            Behavior = await GetBehavior("behavior 1");
            MinRange = await GetParameter<float>("min range");
            MaxRange = await GetParameter<float>("max range");
            SkillTime = await GetParameter<float>("npc skill time");
        }

        public override Task SerializeStart(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            context.MinRange = MinRange;
            context.MaxRange = MaxRange;
            context.SkillTime = SkillTime;
            
            return base.SerializeStart(context, branchContext);
        }
    }
}