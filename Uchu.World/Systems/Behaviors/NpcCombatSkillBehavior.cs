using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class NpcCombatSkillBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters Parameters { get; set; }
    }
    public class NpcCombatSkillBehavior : BehaviorBase<NpcCombatSkillBehaviorExecutionParameters>
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

        protected override void SerializeStart(NpcCombatSkillBehaviorExecutionParameters parameters)
        {
            parameters.NpcContext.MinRange = MinRange;
            parameters.NpcContext.MaxRange = MaxRange;
            parameters.NpcContext.SkillTime = SkillTime;
            parameters.Parameters = Behavior.SerializeStart(parameters.NpcContext, parameters.BranchContext);
        }

        protected override async Task ExecuteStart(NpcCombatSkillBehaviorExecutionParameters parameters)
        {
            await Behavior.ExecuteStart(parameters.Parameters);
        }

        protected override void SerializeSync(NpcCombatSkillBehaviorExecutionParameters parameters)
        {
            Behavior.SerializeSync(parameters.Parameters);
        }
    }
}