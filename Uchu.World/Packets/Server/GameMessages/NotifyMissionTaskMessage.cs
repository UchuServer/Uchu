using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class NotifyMissionTaskMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x00FF;

        public int MissionId { get; set; }
        public int TaskIndex { get; set; }
        public float[] Updates { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteInt(MissionId);
            stream.WriteInt(1 << (TaskIndex + 1));
            stream.WriteByte((byte) Updates.Length);

            foreach (var update in Updates) stream.WriteFloat(update);
        }
    }
}