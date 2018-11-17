using RakDotNet;

namespace Uchu.Core
{
    public class ChangeObjectWorldStateMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x04C7;

        public ObjectWorldState State { get; set; } = ObjectWorldState.Inventory;

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteInt((int) State);
        }
    }
}