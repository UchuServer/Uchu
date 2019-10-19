using System;
using RakDotNet.IO;

namespace Uchu.World
{
    public class UnSmashMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.UnSmash;
        
        public GameObject Builder { get; set; }

        public float Duration { get; set; } = 3f;
        
        public override void SerializeMessage(BitWriter writer)
        {
            if (writer.Flag(Builder != default))
                writer.Write(Builder);
            
            if (writer.Flag(Math.Abs(Duration - 3f) > 0.01f))
                writer.Write(Duration);
        }
    }
}