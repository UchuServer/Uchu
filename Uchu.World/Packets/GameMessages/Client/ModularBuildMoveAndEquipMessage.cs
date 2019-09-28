using RakDotNet.IO;

namespace Uchu.World
{
    public class ModularBuildMoveAndEquipMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ModularBuildMoveAndEquip;

        public Lot Lot;

        public override void Deserialize(BitReader reader)
        {
            Lot = reader.Read<Lot>();
        }
    }
}