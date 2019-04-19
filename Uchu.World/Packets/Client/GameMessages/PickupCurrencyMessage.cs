using System.Numerics;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class PickupCurrencyMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0089;

        public uint Currency;

        public Vector3 Position;
        
        public override void Deserialize(BitStream stream)
        {
            Currency = stream.ReadUInt32();
            Position = new Vector3
            {
                X = stream.ReadFloat(),
                Y = stream.ReadFloat(),
                Z = stream.ReadFloat()
            };
        }
    }
}