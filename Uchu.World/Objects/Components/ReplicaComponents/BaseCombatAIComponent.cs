using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core.Client;
using Uchu.World.Scripting.Native;

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
            var idle = true;
            
            Listen(OnTick, async () =>
            {
                var skillComponent = GameObject.GetComponent<SkillComponent>();

                var destructComponent = GameObject.GetComponent<DestructibleComponent>();

                var rebuild = GameObject.GetComponent<QuickBuildComponent>();
                
                if (!destructComponent.Alive) return;
                
                if (rebuild != default && rebuild.State != RebuildState.Completed) return;
                
                if (Cooldown <= 0)
                {
                    if (!idle)
                    {
                        GameObject.Animate("idle", true);

                        idle = true;
                    }
                    
                    await using var ctx = new CdClientContext();

                    Cooldown = 0.5f;
                    
                    foreach (var skillId in skillComponent.DefaultSkillSet)
                    {
                        var time = await skillComponent.CalculateSkillAsync((int) skillId);

                        if (time.Equals(0)) continue;

                        GameObject.Animate("attack", true);

                        idle = false;
                        
                        var skillInfo = await ctx.SkillBehaviorTable.FirstAsync(
                            s => s.SkillID == skillId
                        );

                        var _ = Task.Run(async () =>
                        {
                            await Task.Delay((int) (time * 1000));
                            
                            if (!idle)
                            {
                                GameObject.Animate("idle", true);

                                idle = true;
                            }
                        });

                        Cooldown = (skillInfo.Cooldown ?? 0.5f) + time;
                        
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
            writer.Write(Target.ObjectId);
        }

        public async Task<GameObject[]> SeekValidTargetsAsync()
        {
            // TODO: Do faction calculations

            return Zone.Players.ToArray();
        }
    }
}