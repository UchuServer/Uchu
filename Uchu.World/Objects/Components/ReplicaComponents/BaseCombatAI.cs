using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class BaseCombatAI : ReplicaComponent
    {
        public bool PerformingAction { get; set; } = false;
        
        public CombatAIAction Action { get; set; }
        
        public GameObject Target { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.BaseCombatAi;
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(PerformingAction);

            if (!PerformingAction) return;
            writer.Write((uint) Action);
            writer.Write(Target);
        }
    }
}