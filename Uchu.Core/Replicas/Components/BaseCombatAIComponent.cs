using RakDotNet;

namespace Uchu.Core
{
    public class BaseCombatAIComponent : ReplicaComponent
    {
        public bool PerformingAction { get; set; } = false;
        public CombatAIAction Action { get; set; } = CombatAIAction.None;
        public long TargetObjectID { get; set; }

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(PerformingAction);

            if (PerformingAction)
            {
                stream.WriteUInt((uint) Action);
                stream.WriteLong(TargetObjectID);
            }
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}