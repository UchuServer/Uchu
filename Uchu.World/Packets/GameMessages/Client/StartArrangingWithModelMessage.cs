using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class StartArrangingWithModelMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.StartArrangingWithModel;

        public Item Item { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Item = reader.ReadGameObject<Item>(Associate.Zone);
            
            Logger.Information($"MODEL: {Item} ; REM: {reader.BaseStream.Length - reader.BaseStream.Position}");
        }
    }
}