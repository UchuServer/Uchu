using System.IO;
using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class NpcExecutionContext : ExecutionContext
    {
        public float MinRange { get; set; }
        
        public float MaxRange { get; set; }
        
        public float SkillTime { get; set; }
        
        public int SkillId { get; set; }
        
        public uint SkillSyncId { get; set; }
        
        public bool FoundTarget { get; set; }

        public bool Alive
        {
            get
            {
                var destructComponent = Associate.GetComponent<DestructibleComponent>();

                var rebuild = Associate.GetComponent<QuickBuildComponent>();
                
                if (!destructComponent.Alive) return false;
                
                if (rebuild != default && rebuild.State != RebuildState.Completed) return false;

                return true;
            }
        }
        
        public NpcExecutionContext(GameObject associate, BitWriter writer, int skillId, uint skillSyncId) : base(associate, default, writer)
        {
            SkillId = skillId;
            SkillSyncId = skillSyncId;
        }

        public void Sync(uint behaviorSyncId)
        {
            Associate.Zone.BroadcastMessage(new EchoSyncSkillMessage
            {
                Associate = Associate,
                SkillHandle = SkillSyncId,
                Content = (Writer.BaseStream as MemoryStream)?.ToArray(),
                Done = true,
                BehaviorHandle = behaviorSyncId
            });
        }

        public NpcExecutionContext Copy()
        {
            return new NpcExecutionContext(Associate, new BitWriter(new MemoryStream()), SkillId, SkillSyncId)
            {
                MaxRange = MaxRange,
                MinRange = MinRange
            };
        }

        public bool IsValidTarget(GameObject gameObject)
        {
            var distance = Vector3.Distance(gameObject.Transform.Position, Associate.Transform.Position);

            return distance <= MaxRange;
        }
    }
}