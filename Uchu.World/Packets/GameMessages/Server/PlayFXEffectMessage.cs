using System;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class PlayFXEffectMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlayFXEffect;

        public int EffectId { get; set; } = -1;

        public string EffectType { get; set; } = "";

        public float Scale { get; set; } = 1;

        public string Name { get; set; } = "";

        public float Priority { get; set; } = 1;
        
        public GameObject Secondary { get; set; }
        
        public bool Serialized { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            if (writer.Flag(EffectId != -1))
                writer.Write(EffectId);

            writer.Write((uint) EffectType.Length);
            writer.WriteString(EffectType, EffectType.Length, true);

            if (writer.Flag(Math.Abs(Scale - 1) > 0.01f))
                writer.Write(Scale);

            writer.Write((uint) Name.Length);
            writer.WriteString(Name, Name.Length);

            if (writer.Flag(Math.Abs(Priority - 1) > 0.01f))
                writer.Write(Priority);

            if (writer.Flag(Secondary != default))
                writer.Write(Secondary);

            writer.WriteBit(Serialized);
        }
    }
}