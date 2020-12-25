using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class CharacterComponent : ReplicaComponent
    {
        public GameObject VehicleObject { get; set; }

        public Character Character { get; set; }

        public bool IsPvP { get; set; }

        public bool IsGameMaster { get; set; }

        public byte GameMasterLevel { get; set; }

        public CharacterActivity Activity { get; set; }

        public long GuildId { get; set; } = -1;

        public string GuildName { get; set; }

        public override ComponentId Id => ComponentId.CharacterComponent;

        public override void Construct(BitWriter writer)
        {
            WritePart1(writer);
            WritePart2(writer);
            WritePart3(writer);

            writer.WriteBit(false);
            writer.WriteBit(false);
            writer.WriteBit(false);
            writer.WriteBit(false);

            writer.Write((uint) Character.HairColor);
            writer.Write((uint) Character.HairStyle);
            writer.Write<uint>(0);
            writer.Write((uint) Character.ShirtColor);
            writer.Write((uint) Character.PantsColor);
            writer.Write<uint>(0);
            writer.Write<uint>(0);
            writer.Write((uint) Character.EyebrowStyle);
            writer.Write((uint) Character.EyeStyle);
            writer.Write((uint) Character.MouthStyle);
            writer.Write((ulong) Character.User.Id);
            writer.Write((ulong) Character.LastActivity);
            writer.Write<ulong>(0);
            writer.Write((ulong) Character.UniverseScore);
            writer.WriteBit(Character.FreeToPlay);

            writer.Write((ulong) Character.TotalCurrencyCollected);
            writer.Write((ulong) Character.TotalBricksCollected);
            writer.Write((ulong) Character.TotalSmashablesSmashed);
            writer.Write((ulong) Character.TotalQuickBuildsCompleted);
            writer.Write((ulong) Character.TotalEnemiesSmashed);
            writer.Write((ulong) Character.TotalRocketsUsed);
            writer.Write((ulong) Character.TotalMissionsCompleted);
            writer.Write((ulong) Character.TotalPetsTamed);
            writer.Write((ulong) Character.TotalImaginationPowerUpsCollected);
            writer.Write((ulong) Character.TotalLifePowerUpsCollected);
            writer.Write((ulong) Character.TotalArmorPowerUpsCollected);
            writer.Write((ulong) Character.TotalDistanceTraveled);
            writer.Write((ulong) Character.TotalSuicides);
            writer.Write((ulong) Character.TotalDamageTaken);
            writer.Write((ulong) Character.TotalDamageHealed);
            writer.Write((ulong) Character.TotalArmorRepaired);
            writer.Write((ulong) Character.TotalImaginationRestored);
            writer.Write((ulong) Character.TotalImaginationUsed);
            writer.Write((ulong) Character.TotalDistanceDriven);
            writer.Write((ulong) Character.TotalTimeAirborne);
            writer.Write((ulong) Character.TotalRacingImaginationPowerUpsCollected);
            writer.Write((ulong) Character.TotalRacingImaginationCratesSmashed);
            writer.Write((ulong) Character.TotalRacecarBoostsActivated);
            writer.Write((ulong) Character.TotalRacecarWrecks);
            writer.Write((ulong) Character.TotalRacingSmashablesSmashed);
            writer.Write((ulong) Character.TotalRacesFinished);
            writer.Write((ulong) Character.TotalFirstPlaceFinishes);
            writer.WriteBit(false);

            writer.WriteBit(Character.LandingByRocket);

            if (Character.LandingByRocket)
            {
                var rocketString = Character.Rocket;
                
                writer.Write((ushort) rocketString.Length);
                writer.WriteString(rocketString, rocketString.Length, true);
            }

            WritePart4(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            WritePart1(writer);
            WritePart2(writer);
            WritePart3(writer);
            WritePart4(writer);
        }

        private void WritePart1(BitWriter writer)
        {
            writer.WriteBit(true);

            var inVehicle = VehicleObject != null;

            writer.WriteBit(inVehicle);

            if (inVehicle) writer.Write(VehicleObject);

            writer.Write<byte>(0);
        }

        private void WritePart2(BitWriter writer)
        {
            var player = (Player) GameObject;
            
            var hasLevel = player.Level != 0;

            writer.WriteBit(hasLevel);

            if (hasLevel) writer.Write((uint) player.Level);
        }

        private static void WritePart3(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.WriteBit(false);
            writer.WriteBit(true);
        }

        private void WritePart4(BitWriter writer)
        {
            writer.WriteBit(true);

            writer.WriteBit(IsPvP);
            writer.WriteBit(IsGameMaster);
            writer.Write<byte>(GameMasterLevel != 1 ? GameMasterLevel : 0);

            writer.WriteBit(false); // ???
            writer.Write<byte>(0); // ???

            writer.WriteBit(true); // Active Activity?
            writer.Write((uint) Activity);

            var hasGuild = GuildId != -1;

            writer.WriteBit(hasGuild);

            if (!hasGuild) return;

            writer.Write(GuildId);
            writer.Write((byte) GuildName.Length);
            writer.WriteString(GuildName, GuildName.Length, true);
            writer.WriteBit(true); // Guild Owner?
            writer.Write(-1); // Guild Creation date
        }
    }
}
