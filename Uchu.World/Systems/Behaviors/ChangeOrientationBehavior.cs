using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class ChangeOrientationBehavior : BehaviorBase
    {
        private float Angle { get; set; }
        private float Duration { get; set; }
        private bool OrientCaster { get; set; }
        private bool Relative { get; set; }
        private bool ToAngle { get; set; }
        private bool ToPoint { get; set; }
        private bool ToTarget { get; set; }
        public override BehaviorTemplateId Id => BehaviorTemplateId.ChangeOrientation;
        public override async Task BuildAsync()
        {
            OrientCaster = await GetParameter<int>("orient_caster") == 1;
            ToTarget = await GetParameter<int>("to_target") == 1;
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            parameters.NpcContext.Associate.Transform.LookAt(parameters.BranchContext.Target.Transform.Position);
            //the variables seem to do nothing
            /*
            if (OrientCaster)
            {
                if (ToTarget)
                {
                    parameters.NpcContext.Associate.Transform.LookAt(parameters.BranchContext.Target.Transform.Position);
                }
            }
            else
            {
                if (ToTarget)
                {
                    parameters.NpcContext.Associate.Transform.LookAt(parameters.BranchContext.Target.Transform.Position);
                }
            }
            */
        }
    }
}