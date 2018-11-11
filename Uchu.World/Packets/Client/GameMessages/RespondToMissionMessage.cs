using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class RespondToMissionMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x00F9;

        public int MissionId { get; set; }

        public long PlayerObjectId { get; set; }

        public long ReceiverObjectId { get; set; }

        public int RewardItem { get; set; }

        public override void Deserialize(BitStream stream)
        {
            MissionId = stream.ReadInt();
            PlayerObjectId = stream.ReadLong();
            ReceiverObjectId = stream.ReadLong();
            RewardItem = stream.ReadBit() ? stream.ReadInt() : -1;
        }
    }
}