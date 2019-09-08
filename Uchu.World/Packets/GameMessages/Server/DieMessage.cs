using RakDotNet;
using RakDotNet.IO;

namespace Uchu.World
{
    public class DieMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.Die;
        
        public bool ClientDeath { get; set; }

        public bool SpawnLoot { get; set; } = true;

        public string DeathType { get; set; } = "";

        public float DirectionRelativeAngleXz { get; set; }

        public float DirectionRelativeAngleY { get; set; }

        public float DirectionRelativeForce { get; set; }
        
        public int KillType { get; set; }
        
        public GameObject Killer { get; set; }
        
        public GameObject LootOwner { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(ClientDeath);
            writer.WriteBit(SpawnLoot);

            writer.WriteString(DeathType);

            writer.Write(DirectionRelativeAngleXz);
            writer.Write(DirectionRelativeAngleY);
            writer.Write(DirectionRelativeForce);

            var hasKillType = KillType != default;
            writer.WriteBit(hasKillType);
            if (hasKillType) writer.Write(KillType);

            writer.Write(Killer);

            var hasLootOwner = LootOwner != default;
            writer.WriteBit(hasLootOwner);
            if (hasLootOwner) writer.Write(LootOwner);
        }
    }
}