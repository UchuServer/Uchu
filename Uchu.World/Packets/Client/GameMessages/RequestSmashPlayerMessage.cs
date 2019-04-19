using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class RequestSmashPlayerMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x04b2;

        public override void Deserialize(BitStream stream)
        {
            
        }
    }
}