using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class DieMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.Die;

        public bool ClientDeath { get; set; } = false;

        public bool SpawnLoot { get; set; } = true;

        public string DeathType { get; set; } = "";

        public float DirectionRelativeAngleXz { get; set; } = 0.0f;

        public float DirectionRelativeAngleY { get; set; } = 0.0f;

        public float DirectionRelativeForce { get; set; } = 0.0f;

        public int KillType { get; set; } = 0;

        public GameObject Killer { get; set; }

        public GameObject LootOwner { get; set; } = default;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(ClientDeath);
            writer.WriteBit(SpawnLoot);

            DeathType ??= "";
            writer.Write((uint) DeathType.Length);
            writer.WriteString(DeathType, DeathType.Length, true);

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