using System.Numerics;
using System.Threading.Tasks;
using RakDotNet.IO;

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

        public override BehaviorExecutionParameters DeserializeStart(BitReader reader, ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            reader.ReadBit();
            return base.DeserializeStart(reader, context, branchContext);
        }

        public override BehaviorExecutionParameters SerializeStart(BitWriter writer, NpcExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            writer.WriteBit(false);
            if (branchContext.Target is Player target)
            {
                var targetDirection = context.Associate.Transform.Position - target.Transform.Position;
                var rotation = targetDirection.QuaternionLookRotation(Vector3.UnitY);
                
                // Forward, currently unused
                _ = rotation.VectorMultiply(Vector3.UnitX);

                // Handled in the background
                Task.Run(() =>
                {
                    target.Message(new KnockbackMessage
                    {
                        Associate = target,
                        Caster = context.Associate,
                        Originator = context.Associate,
                        KnockbackTime = 0,
                        Vector = Vector3.UnitY * Strength
                    });
                });
            }

            return base.SerializeStart(writer, context, branchContext);
        }
    }
}