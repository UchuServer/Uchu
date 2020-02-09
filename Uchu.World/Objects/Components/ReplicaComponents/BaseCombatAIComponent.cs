using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core.Client;
using Uchu.World.AI;

namespace Uchu.World
{
    public class BaseCombatAiComponent : ReplicaComponent
    {
        public bool PerformingAction { get; set; }

        public CombatAiAction Action { get; set; }

        public GameObject Target { get; set; }

        public override ComponentId Id => ComponentId.BaseCombatAIComponent;
        
        public float Cooldown { get; set; }
        
        public List<NpcSkillEntry> SkillEntries { get; set; }

        private SkillComponent SkillComponent { get; set; }
        
        private DestructibleComponent DestructibleComponent { get; set; }
        
        private QuickBuildComponent QuickBuildComponent { get; set; }
        
        public BaseCombatAiComponent()
        {
            Listen(OnStart, async () =>
            {
                SkillEntries = new List<NpcSkillEntry>();
                
                SkillComponent = GameObject.GetComponent<SkillComponent>();

                DestructibleComponent = GameObject.GetComponent<DestructibleComponent>();

                QuickBuildComponent = GameObject.GetComponent<QuickBuildComponent>();

                foreach (var skillId in SkillComponent.DefaultSkillSet)
                {
                    SkillEntries.Add(new NpcSkillEntry
                    {
                        SkillId = skillId,
                        Cooldown = false
                    });
                }
            });
            
            Listen(OnTick, async () =>
            {
                if (GameObject.Lot == 6007 || GameObject.Lot == 6366) return; // TODO: Remove
                
                if (!DestructibleComponent.Alive) return;
                
                if (QuickBuildComponent != default && QuickBuildComponent.State != RebuildState.Completed) return;
                
                if (Cooldown <= 0)
                {
                    await using var ctx = new CdClientContext();

                    Cooldown = 1f;
                    
                    foreach (var entry in SkillEntries.Where(s => !s.Cooldown))
                    {
                        var time = await SkillComponent.CalculateSkillAsync((int) entry.SkillId);

                        if (time.Equals(0)) continue;

                        entry.Cooldown = true;
                        
                        var skillInfo = await ctx.SkillBehaviorTable.FirstAsync(
                            s => s.SkillID == entry.SkillId
                        );

                        var _ = Task.Run(async () =>
                        {
                            var cooldown = (skillInfo.Cooldown ?? 1f) + time;
                            
                            await Task.Delay((int) cooldown * 1000);

                            entry.Cooldown = false;
                        });

                        Cooldown += time;
                        
                        break;
                    }
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
            writer.Write(Target);
        }

        public async Task<GameObject[]> SeekValidTargetsAsync()
        {
            // TODO: Do faction calculations

            return Zone.Players.ToArray();
        }
    }
}