using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class RequestLinkedMissionMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0203;

        public long PlayerObjectId { get; set; }
        public int MissionId { get; set; }
        public bool OfferedMission { get; set; }

        public override void Deserialize(BitStream stream)
        {
            PlayerObjectId = stream.ReadLong();
            MissionId = stream.ReadInt();
            OfferedMission = stream.ReadBit();
        }
    }
}