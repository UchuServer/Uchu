using System;
using RakDotNet;
using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class Renderer : ReplicaComponent
    {
        public bool Disabled { get; set; }

        public Effect[] Effects { get; set; } = new Effect[0];
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Render;
        
        public override void Construct(BitWriter writer)
        {
            if (Disabled) return;
            writer.Write((uint) Effects.Length);

            foreach (var effect in Effects)
            {
                writer.Write(effect);
            }
        }

        public override void Serialize(BitWriter writer)
        {
        }
        
        public class Effect : ISerializable
        {
            public string Name { get; set; }

            public uint EffectId { get; set; }

            public string Type { get; set; }

            public float Scale { get; set; }

            public long Secondary { get; set; }

            public void Serialize(BitWriter writer)
            {
                writer.Write((byte) Name.Length);
                writer.WriteString(Name, Name.Length, false);

                writer.Write(EffectId);

                writer.Write((byte) Type.Length);
                writer.WriteString(Type, Type.Length, true);

                writer.Write(Scale);

                writer.Write(Secondary);
            }

            public void Deserialize(BitReader reader)
            {
                throw new NotSupportedException();
            }
        }
    }
}