using System;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class PlayAnimationMessage : ServerGameMessage
    {
        public const float SecondaryPriority = 0.400000005960465f;

        public override GameMessageId GameMessageId => GameMessageId.PlayAnimation;
        
        public string AnimationsId { get; set; }

        public bool ExpectAnimationToExist { get; set; } = true;
        
        public bool PlayImmediate { get; set; }
        
        public bool TriggerOnCompleteMessage { get; set; }

        public float Priority { get; set; } = SecondaryPriority;

        public float Scale { get; set; } = 1f;
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((uint) AnimationsId.Length);
            writer.WriteString(AnimationsId, AnimationsId.Length, true);

            writer.WriteBit(ExpectAnimationToExist);
            writer.WriteBit(PlayImmediate);
            writer.WriteBit(TriggerOnCompleteMessage);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (writer.Flag(Priority != SecondaryPriority))
            {
                writer.Write(Priority);
            }

            if (writer.Flag(Math.Abs(Scale - 1) > 0.01f))
            {
                writer.Write(Scale);
            }
        }
    }
}