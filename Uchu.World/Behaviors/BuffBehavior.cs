using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class BuffBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Buff;
        
        public int Life { get; set; }
        
        public int Armor { get; set; }
        
        public int Imagination { get; set; }
        
        public float RunSpeed { get; set; }
        
        public float AttackSpeed { get; set; }
        
        public float Brain { get; set; }
        
        public override async Task BuildAsync()
        {
            Life = await GetParameter<int>("life");
            Armor = await GetParameter<int>("armor");
            Imagination = await GetParameter<int>("imag");
            RunSpeed = await GetParameter<int>("run_speed");
            AttackSpeed = await GetParameter<int>("attack_speed");
            Brain = await GetParameter<int>("brain");
        }

        public override Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            (context.Associate as Player)?.SendChatMessage($"Adding: L: {Life}, A: {Armor}, I: {Imagination}");
            
            if (!context.Associate.TryGetComponent<Stats>(out var stats)) return Task.CompletedTask;

            stats.MaxHealth += (uint) Life;
            stats.MaxArmor += (uint) Armor;
            stats.MaxImagination += (uint) Imagination;

            return Task.CompletedTask;
        }

        public override Task DismantleAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            (context.Associate as Player)?.SendChatMessage($"Removing: L: {Life}, A: {Armor}, I: {Imagination}");
            
            if (!context.Associate.TryGetComponent<Stats>(out var stats)) return Task.CompletedTask;

            stats.MaxHealth -= (uint) Life;
            stats.MaxArmor -= (uint) Armor;
            stats.MaxImagination -= (uint) Imagination;
            
            return Task.CompletedTask;
        }
    }
}