using System;
using RakDotNet.IO;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct RemoveBuffMessage
    {

        public GameObject Associate { get; set; }
        
        public GameMessageId GameMessageId => GameMessageId.RemoveBuff;

        public bool FromRemoveBehavior { get; set; }

        public bool FromUnequip { get; set; }

        public bool RemoveImmunity { get; set; }

        public uint BuffID { get; set; }
        
    }
}