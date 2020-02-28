using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Scripting.Utils;
using RakDotNet.IO;
using Uchu.Core.Client;
using Uchu.World.AI;

namespace Uchu.World
{
    public class BaseCombatAiComponent : ReplicaComponent
    {
        public bool PerformingAction { get; set; } = false;

        public CombatAiAction Action { get; set; }

        public GameObject Target { get; set; }

        public override ComponentId Id => ComponentId.BaseCombatAIComponent;
        
        public float Cooldown { get; set; }
        
        public bool AbilityDowntime { get; set; }
        
        public List<NpcSkillEntry> SkillEntries { get; set; }

        private SkillComponent SkillComponent { get; set; }
        
        private DestructibleComponent DestructibleComponent { get; set; }
        
        private QuickBuildComponent QuickBuildComponent { get; set; }
        
        private Stats Stats { get; set; }

        public bool Enabled { get; set; } = true;
        
        private Vector3 Origin { get; set; }
        
        public BaseCombatAiComponent()
        {
            Listen(OnStart, () =>
            {
                SkillEntries = new List<NpcSkillEntry>();
                
                SkillComponent = GameObject.GetComponent<SkillComponent>();

                DestructibleComponent = GameObject.GetComponent<DestructibleComponent>();

                QuickBuildComponent = GameObject.GetComponent<QuickBuildComponent>();

                Stats = GameObject.GetComponent<Stats>();

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
                if (!Enabled) return;
                
                if (!DestructibleComponent.Alive) return;
                
                if (QuickBuildComponent != default && QuickBuildComponent.State != RebuildState.Completed) return;
                
                if (Cooldown <= 0)
                {
                    await using var ctx = new CdClientContext();

                    AbilityDowntime = false;
                    
                    Cooldown = 1f;
                    
                    foreach (var entry in SkillEntries.Where(s => !s.Cooldown))
                    {
                        var time = await SkillComponent.CalculateSkillAsync((int) entry.SkillId);

                        if (time.Equals(0)) continue;

                        AbilityDowntime = true;
                        
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

        public GameObject[] SeekValidTargets()
        {
            // TODO: Optimize

            if (Stats.Factions.Length == default) return new GameObject[0];

            var entries = Zone.Objects.OfType<Stats>();

            var targets = new List<GameObject>();
            
            foreach (var entry in entries.Where(e => e.Factions.Length != default && e.Health > 0))
            {
                if (Stats.Enemies.Contains(entry.Factions.First()))
                {
                    targets.Add(entry.GameObject);
                }
            }

            return targets.ToArray();
        }
    }
}