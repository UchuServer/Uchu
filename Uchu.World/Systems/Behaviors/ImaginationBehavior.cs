using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class ImaginationBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Imagination;

        private int Imagination { get; set; }
        
        public override async Task BuildAsync()
        {
            Imagination = await GetParameter<int>("imagination");
        }

        public override Task ExecuteStart(BehaviorExecutionParameters parameters)
        {
            if (parameters.BranchContext.Target.TryGetComponent<Stats>(out var stats))
                stats.Imagination = (uint) ((int) stats.Imagination + Imagination);
            return Task.CompletedTask;
        }
    }
}