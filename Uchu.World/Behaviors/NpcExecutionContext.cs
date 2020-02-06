using System.IO;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class NpcExecutionContext : ExecutionContext
    {
        public float MinRange { get; set; }
        
        public float MaxRange { get; set; }
        
        public bool Start { get; set; }
        
        public int SkillId { get; set; }
        
        public NpcExecutionContext(GameObject associate, BitWriter writer, int skillId) : base(associate, default, writer)
        {
            Start = true;
            SkillId = skillId;
        }

        public NpcExecutionContext Flush()
        {
            if (Start)
            {
                Associate.Zone.BroadcastMessage(new EchoStartSkillMessage
                {
                    SkillId = SkillId,
                    Associate = Associate,
                    CastType = (int) SkillCastType.OnUse,
                    Content = (Writer.BaseStream as MemoryStream)?.ToArray()
                });
            }

            return this;
        }
    }
}