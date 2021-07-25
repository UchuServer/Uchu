using System;
using RakDotNet.IO;

namespace Uchu.World
{
    public class RemoveBuffMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RemoveBuff;

        public bool FromRemoveBehavior { get; set; }

        public bool FromUnequip { get; set; }

        public bool RemoveImmunity { get; set; }

        public uint BuffID { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(FromRemoveBehavior);
            writer.WriteBit(FromUnequip);
            writer.WriteBit(RemoveImmunity);
            writer.Write(BuffID);
        }
    }
}