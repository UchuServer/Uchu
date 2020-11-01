using System.Numerics;
using InfectedRose.Lvl;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class PropertyRentalResponseMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PropertyRentalResponse;

        public uint CloneID;
        public PropertyRentalResponseCode Code;
        public long PropertyID;
        public long RentDue;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(CloneID);
            writer.Write((int)Code);
            writer.Write(PropertyID);
            writer.Write(RentDue);
        }
    }
}
