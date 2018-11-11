using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class MissionDialogueOkMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0208;

        public bool IsComplete { get; set; }

        public int MissionState { get; set; }

        public int MissionId { get; set; }

        public long ResponderId { get; set; }

        public override void Deserialize(BitStream stream)
        {
            IsComplete = stream.ReadBit();
            MissionState = stream.ReadInt();
            MissionId = stream.ReadInt();
            ResponderId = stream.ReadLong();
        }
    }
}