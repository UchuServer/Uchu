using RakDotNet;
using RakDotNet.IO;

namespace Uchu.World
{
    public class RequestDieMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestDie;

        public bool UnknownFlag { get; set; }

        public string DeathType { get; set; } = "";

        public float DirectionRelativeAngleXz { get; set; }

        public float DirectionRelativeAngleY { get; set; }

        public float DirectionRelativeForce { get; set; }

        public int KillType { get; set; }

        public GameObject Killer { get; set; }

        public GameObject LootOwner { get; set; }

        public override void Deserialize(BitReader reader)
        {
            UnknownFlag = reader.ReadBit();

            DeathType = reader.ReadString();

            DirectionRelativeAngleXz = reader.Read<float>();
            DirectionRelativeAngleY = reader.Read<float>();
            DirectionRelativeForce = reader.Read<float>();

            if (reader.ReadBit()) KillType = reader.Read<int>();

            Killer = reader.ReadGameObject(Associate.Zone);
            LootOwner = reader.ReadGameObject(Associate.Zone);
        }
    }
}