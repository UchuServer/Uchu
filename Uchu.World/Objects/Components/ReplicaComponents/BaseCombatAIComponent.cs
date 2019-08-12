using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    public class BaseCombatAiComponent : ReplicaComponent
    {
        public bool PerformingAction { get; set; } = false;
        
        public CombatAiAction Action { get; set; }
        
        public GameObject Target { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.BaseCombatAi;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

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