using RakDotNet;

namespace Uchu.Core
{
    public class UIMessageToClientMessage<T> : ServerGameMessage
    {
        public override ushort GameMessageId => 0x04A0;

        public AMF3<T> Arguments { get; set; }
        public string MessageName { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteSerializable(Arguments);
            stream.WriteUInt((uint) MessageName.Length);
            stream.WriteString(MessageName, MessageName.Length);
        }
    }
}