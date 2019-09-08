using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class PickupCurrencyMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PickupCurrency;
        
        public uint Currency { get; set; }
        
        public Vector3 Position { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Currency = reader.Read<uint>();

            Position = reader.Read<Vector3>();
        }
    }
}