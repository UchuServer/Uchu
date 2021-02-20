using IronPython.Runtime.Types;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class AddPetToPlayerMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.AddPetToPlayer;

        public int iElementalType;
        public string name;
        public GameObject petDBID;
        public Lot PetLOT;
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write<int>(iElementalType);

            writer.Write((uint)name.Length);
            writer.WriteString(name, name.Length, true);

            writer.Write(petDBID);
            writer.Write(PetLOT);
        }
    }
}