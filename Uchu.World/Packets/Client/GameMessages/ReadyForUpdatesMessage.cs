using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class ReadyForUpdatesMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0378;

        public override void Deserialize(BitStream stream)
        {
            
        }
    }
}