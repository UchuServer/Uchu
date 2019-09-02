using RakDotNet.IO;

namespace Uchu.World
{
    public class PickupItemMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x8B;

        public GameObject Loot { get; set; }
        
        public Player Player { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Loot = reader.ReadGameObject(Associate.Zone);
            Player = reader.ReadGameObject(Associate.Zone) as Player;
        }
    }
}