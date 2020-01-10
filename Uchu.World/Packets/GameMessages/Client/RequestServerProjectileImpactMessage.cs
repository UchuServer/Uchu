using System.Linq;
using RakDotNet.IO;

namespace Uchu.World
{
    public class RequestServerProjectileImpactMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestServerProjectileImpact;

        public long Projectile { get; set; }
        
        public GameObject Target { get; set; }
        
        public byte[] Data { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit())
            {
                Projectile = reader.Read<long>();
            }

            if (reader.ReadBit())
            {
                Target = reader.ReadGameObject(Associate.Zone);
            }

            Data = new byte[reader.Read<uint>()];

            for (var i = 0; i < Data.Length; i++)
            {
                Data[i] = reader.Read<byte>();
            }
        }
    }
}