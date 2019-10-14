using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class FireServerEventMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.FireEventServerSide;
        
        public string Arguments { get; set; }
        
        public int[] Parameters { get; set; }
        
        public GameObject Sender { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Arguments = reader.ReadString((int) reader.Read<uint>(), true);

            Parameters = new int[3];

            for (var i = 0; i < 3; i++)
            {
                Parameters[i] = reader.ReadBit() ? reader.Read<int>() : -1;
            }

            Sender = reader.ReadGameObject(Associate.Zone);
        }
    }
}