using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class RequestDieMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0026;
        
        public override void Deserialize(BitStream stream)
        {
            
        }
    }
}