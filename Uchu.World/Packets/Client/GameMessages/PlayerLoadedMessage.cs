using System;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class PlayerLoadedMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x01F9;

        public long PlayerID;
        
        public override void Deserialize(BitStream stream)
        {
            PlayerID = stream.ReadInt64();
        }
    }
}