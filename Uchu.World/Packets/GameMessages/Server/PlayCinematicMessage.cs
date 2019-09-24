using System;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class PlayCinematicMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlayCinematic;

        public bool AllowGhostUpdates { get; set; } = true;
        
        public bool CloseMultiInteract { get; set; }
        
        public bool SendServerNotify { get; set; }
        
        public bool UseControlledObjectForAudioLister { get; set; }
        
        public int EndBehavior { get; set; }
        
        public bool HidePlayerDuringCinematic { get; set; }

        public float LeadIn { get; set; } = -1;
        
        public bool LeavePlayerLockedWhenFinished { get; set; }

        public bool LockPlayer { get; set; } = true;
        
        public string Path { get; set; }
        
        public bool Result { get; set; }
        
        public bool SkipIfSamePath { get; set; }
        
        public float StartTimeAdvance { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(AllowGhostUpdates);
            writer.WriteBit(CloseMultiInteract);
            writer.WriteBit(SendServerNotify);
            writer.WriteBit(UseControlledObjectForAudioLister);

            var hasEndBehavior = EndBehavior != 0;
            writer.WriteBit(hasEndBehavior);
            if (hasEndBehavior) writer.Write(EndBehavior);

            writer.WriteBit(HidePlayerDuringCinematic);

            var hasLeadIn = Math.Abs(LeadIn - -1) > 0.01f;
            writer.WriteBit(hasLeadIn);
            if (hasLeadIn) writer.Write(LeadIn);

            writer.WriteBit(LeavePlayerLockedWhenFinished);
            writer.WriteBit(LockPlayer);

            writer.Write((uint) Path.Length);
            writer.WriteString(Path, Path.Length, true);

            writer.WriteBit(Result);
            writer.WriteBit(SkipIfSamePath);

            writer.Write(StartTimeAdvance);
        }
    }
}