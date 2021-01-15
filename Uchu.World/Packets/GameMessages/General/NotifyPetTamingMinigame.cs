using System.Numerics;
using InfectedRose.Core;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class NotifyPetTamingMinigame : GeneralGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.NotifyPetTamingMinigame;

        public ObjectId PetID { get; set; }
        public ObjectId PlayerTamingID { get; set; }
        public bool bForceTeleport { get; set; }
        public NotifyType notifyType { get; set; }
        public Vector3 petsDestPos { get; set; }
        public Vector3 telePos { get; set; }
        public Quaternion teleRot { get; set; } = Quaternion.Identity;
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write<ulong>(PetID);
            writer.Write<ulong>(PlayerTamingID);
            writer.WriteBit(bForceTeleport);
            writer.Write<uint>((uint) notifyType);
            writer.Write<Vector3>(petsDestPos);
            writer.Write<Vector3>(telePos);
            
            writer.WriteBit(teleRot != Quaternion.Identity);
            if (teleRot != Quaternion.Identity)
            {
                writer.Write<Quaternion>(teleRot);
            }
        }

        public override void Deserialize(BitReader reader)
        {
            // Implement it 
        }
    }
}