using InfectedRose.Core;
using RakDotNet.IO;
using System.Numerics;

namespace Uchu.World
{
    public class ZonePropertyModelRemovedWhileEquipped : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ZonePropertyModelRemovedWhileEquipped;

        public long playerID = 0;
        public long propertyID = 0;

        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit()) playerID = reader.Read<long>();
            if (reader.ReadBit()) propertyID = reader.Read<long>();
        }
    }
}
