using System;
using RakDotNet.IO;

namespace Uchu.World
{
    public class SetGravityScaleMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetGravityScale;

        public float Scale { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(Math.Clamp(Scale, 0, 2));
        }
    }
}