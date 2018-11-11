using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class OfferMissionMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x00F8;

        public int MissionId { get; set; }
        public long OffererObjectId { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteInt(MissionId);
            stream.WriteLong(OffererObjectId);
        }
    }
}