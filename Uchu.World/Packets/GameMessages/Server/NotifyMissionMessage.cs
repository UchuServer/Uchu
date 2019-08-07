using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class NotifyMissionMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0xFE;
        
        public int MissionId { get; set; }

        public MissionState MissionState { get; set; } = MissionState.Active;
        
        public bool SendingRewards { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(MissionId);

            writer.Write((int) MissionState);

            writer.WriteBit(SendingRewards);
        }
    }
}