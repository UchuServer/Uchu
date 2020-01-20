using InfectedRose.Lvl;
using RakDotNet.IO;

namespace Uchu.World
{
    public class MatchRequestMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.MatchRequest;

        public int Type { get; set; }
        
        public int Value { get; set; }
        
        public GameObject Activator { get; set; }
        
        public LegoDataList LegoDataList { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Type = reader.Read<int>();

            Value = reader.Read<int>();

            Activator = reader.ReadGameObject(Associate.Zone);
            
            // TODO: Additional LDL
        }
    }
}