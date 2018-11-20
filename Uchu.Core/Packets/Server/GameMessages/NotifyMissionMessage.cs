using RakDotNet;

namespace Uchu.Core
{
    public class NotifyMissionMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x00FE;

        public int MissionId { get; set; }
        public MissionState MissionState { get; set; } = MissionState.Active;
        public bool SendingRewards { get; set; } = false;

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteInt(MissionId);
            stream.WriteInt((int) MissionState);
            stream.WriteBit(SendingRewards);
        }
    }
}