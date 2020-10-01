using System.Numerics;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class KnockbackBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Knockback;

        private float Strength { get; set; }

        private float Time { get; set; }
        
        public override async Task BuildAsync()
        {
            Strength = await GetParameter<float>("strength");
            Time = await GetParameter<int>("time_ms");
        }

        public override BehaviorExecutionParameters DeserializeStart(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            context.Reader.ReadBit();
            return base.DeserializeStart(context, branchContext);
        }

        public override Task SerializeStart(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            context.Writer.WriteBit(false);
            if (branchContext.Target is Player target)
            {
                var targetDirection = context.Associate.Transform.Position - target.Transform.Position;
                var rotation = targetDirection.QuaternionLookRotation(Vector3.UnitY);
                var forward = rotation.VectorMultiply(Vector3.UnitX);

                target.Message(new KnockbackMessage
                {
                    Associate = target,
                    Caster = context.Associate,
                    Originator = context.Associate,
                    KnockbackTime = 0,
                    Vector = Vector3.UnitY * Strength
                });
            }

            return Task.CompletedTask;
        }
    }
}