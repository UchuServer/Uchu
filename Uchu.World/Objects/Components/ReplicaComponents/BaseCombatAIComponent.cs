using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World
{
    public class BaseCombatAiComponent : ReplicaComponent
    {
        public bool PerformingAction { get; set; }

        public CombatAiAction Action { get; set; }

        public GameObject Target { get; set; }

        public override ComponentId Id => ComponentId.BaseCombatAIComponent;
        
        public float Cooldown { get; set; }
        
        public BaseCombatAiComponent()
        {
            Listen(OnTick, async () =>
            {
                var skillComponent = GameObject.GetComponent<SkillComponent>();

                if (Cooldown <= 0)
                {
                    foreach (var skillId in skillComponent.DefaultSkillSet)
                    {
                        await skillComponent.CalculateSkillAsync((int) skillId);
                    }

                    Cooldown = 4;
                }

                Cooldown -= Zone.DeltaTime;
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

        public async Task<GameObject[]> SeekValidTargetsAsync()
        {
            // TODO: Do faction calculations

            return Zone.Players.ToArray();
        }
    }
}