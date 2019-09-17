using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class PetComponent : ReplicaComponent
    {
        public GameObject Owner { get; set; }

        public uint ModerationStatus { get; set; }

        public GameObject PetInteractionObject { get; set; }

        public override ReplicaComponentsId Id => ReplicaComponentsId.Pet;

        public override void FromLevelObject(LevelObject levelObject)
        {
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.Write<uint>(0);
            writer.Write<uint>(0);

            var hasPetInteraction = PetInteractionObject != null;
            writer.WriteBit(hasPetInteraction);
            if (hasPetInteraction) writer.Write(PetInteractionObject);

            var hasOwner = Owner != null;
            writer.WriteBit(hasOwner);
            if (hasOwner) writer.Write(Owner);

            writer.WriteBit(true);
            writer.Write(ModerationStatus);

            writer.Write((byte) GameObject.Name.Length);
            writer.WriteString(GameObject.Name, GameObject.Name.Length, true);

            if (hasOwner)
            {
                writer.Write((byte) Owner.Name.Length);
                writer.WriteString(Owner.Name, Owner.Name.Length, true);
            }
            else
            {
                writer.Write<byte>(0);
            }
        }
    }
}