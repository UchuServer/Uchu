using InfectedRose.Core;
using RakDotNet.IO;
using System.Diagnostics.Contracts;
using Uchu.Core;

namespace Uchu.World
{
    public class PropertyModerationAction : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PropertyModerationAction;

        public long characterID = 0;
        public string info;
        public int newModerationStatus = -1;

        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit()) characterID = reader.Read<long>();
            reader.ReadString((int)reader.Read<uint>(), true);
            if (reader.ReadBit()) newModerationStatus = reader.Read<int>();
        }
    }
}
