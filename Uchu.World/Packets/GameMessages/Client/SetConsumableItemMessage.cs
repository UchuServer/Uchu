using RakDotNet.IO;

namespace Uchu.World
{
    public class SetConsumableItemMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetConsumableItem;
        
        public Lot Lot { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Lot = reader.Read<Lot>();
        }
    }
}