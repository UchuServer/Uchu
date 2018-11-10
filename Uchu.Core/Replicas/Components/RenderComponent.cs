using System;
using RakDotNet;

namespace Uchu.Core
{
    public class RenderComponent : ReplicaComponent
    {
        public bool Disabled { get; set; } = false;
        public Effect[] Effects { get; set; } = new Effect[0];

        public override void Construct(BitStream stream)
        {
            if (!Disabled)
            {
                stream.WriteUInt((uint) Effects.Length);

                foreach (var effect in Effects) stream.WriteSerializable(effect);
            }
        }

        public override void Serialize(BitStream stream)
        {
        }

        public class Effect : ISerializable
        {
            public string Name { get; set; }

            public uint EffectId { get; set; }

            public string Type { get; set; }

            public float Scale { get; set; }

            public long Secondary { get; set; }

            public void Serialize(BitStream stream)
            {
                stream.WriteByte((byte) Name.Length);
                stream.WriteString(Name, Name.Length, false);

                stream.WriteUInt(EffectId);

                stream.WriteByte((byte) Type.Length);
                stream.WriteString(Type, Type.Length, true);

                stream.WriteFloat(Scale);

                stream.WriteLong(Secondary);
            }

            public void Deserialize(BitStream stream)
            {
                throw new NotSupportedException();
            }
        }
    }
}