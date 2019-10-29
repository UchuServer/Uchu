using System;
using RakDotNet.IO;

namespace Uchu.World
{
    public class AddSkillMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.AddSkill;

        public int AiCombatWeight { get; set; }
        
        public bool FromSkillSet { get; set; }
        
        public SkillCastType CastType { get; set; }

        public float TimeSecs { get; set; } = -1;

        public int TimesCanCast { get; set; } = -1;
        
        public uint SkillId { get; set; }

        public BehaviorSlot SlotId { get; set; } = BehaviorSlot.None;

        public bool Temporary { get; set; } = true;
        
        public override void SerializeMessage(BitWriter writer)
        {
            if (writer.Flag(AiCombatWeight != default))
                writer.Write(AiCombatWeight);

            writer.WriteBit(FromSkillSet);

            if (writer.Flag(CastType != default))
                writer.Write(CastType);

            if (writer.Flag(Math.Abs(TimeSecs + 1) > 0.01f))
                writer.Write(TimeSecs);

            if (writer.Flag(TimesCanCast != -1))
                writer.Write(TimesCanCast);

            writer.Write(SkillId);

            if (writer.Flag(SlotId != BehaviorSlot.None))
                writer.Write((int) SlotId);

            writer.Write(Temporary);
        }
    }
}