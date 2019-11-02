using RakDotNet.IO;
using Uchu.World.Experimental;

namespace Uchu.World
{
    public class BaseCombatAiComponent : ReplicaComponent
    {
        public bool PerformingAction { get; set; }

        public CombatAiAction Action { get; set; }

        public GameObject Target { get; set; }

        public override ComponentId Id => ComponentId.BaseCombatAIComponent;

        protected BaseCombatAiComponent()
        {
            OnStart.AddListener(() =>
            {
                GameObject.AddComponent<EnemyAi>();
            });
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
            writer.Write(Target.ObjectId);
        }
    }
}