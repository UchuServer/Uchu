using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class ClientItemConsumedMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x1ac;

        public long Item;
        
        public override void Deserialize(BitStream stream)
        {
            Item = stream.ReadInt64();
        }
    }
}