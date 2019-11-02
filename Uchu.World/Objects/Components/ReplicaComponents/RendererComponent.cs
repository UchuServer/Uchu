using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class RendererComponent : ReplicaComponent
    {
        public bool Disabled { get; set; }

        public Effect[] Effects { get; set; } = new Effect[0];

        public override ComponentId Id => ComponentId.RenderComponent;

        protected RendererComponent()
        {
            OnStart.AddListener(() =>
            {
                if (GameObject.Settings.TryGetValue("renderDisabled", out var disabled)) Disabled = (bool) disabled;
                else Disabled = false;
            });
        }

        public override void Construct(BitWriter writer)
        {
            if (Disabled) return;

            writer.Write((uint) Effects.Length);

            foreach (var effect in Effects) writer.Write(effect);
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
                writer.WriteString(Name, Name.Length);

                writer.Write(EffectId);

                writer.Write((byte) Type.Length);
                writer.WriteString(Type, Type.Length, true);

                writer.Write(Scale);

                writer.Write(Secondary);
            }
        }
    }
}