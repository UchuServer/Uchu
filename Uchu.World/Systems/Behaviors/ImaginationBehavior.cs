using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class ImaginationBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Imagination;

        private int Imagination { get; set; }
        
        public override void Build()
        {
            Imagination = GetParameter<int>("imagination");
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            if (parameters.BranchContext.Target.TryGetComponent<DestroyableComponent>(out var stats))
                stats.Imagination = (uint) ((int) stats.Imagination + Imagination);
        }
    }
}