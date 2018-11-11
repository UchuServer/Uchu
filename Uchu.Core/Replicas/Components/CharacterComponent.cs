using RakDotNet;

namespace Uchu.Core
{
    public class CharacterComponent : ReplicaComponent
    {
        public long VehicleObjectId { get; set; } = -1;

        public uint Level { get; set; } = 0;

        public Character Character { get; set; }

        public string Rocket { get; set; } = null;

        public bool IsPvP { get; set; } = false;

        public bool IsGM { get; set; } = false;

        public byte GMLevel { get; set; } = 0;

        public CharacterActivity Activity { get; set; } = CharacterActivity.None;

        public long GuildId { get; set; } = -1;

        public string GuildName { get; set; }

        private void _writePart1(BitStream stream)
        {
            stream.WriteBit(true);

            var inVehicle = VehicleObjectId != -1;

            stream.WriteBit(inVehicle);

            if (inVehicle)
            {
                stream.WriteLong(VehicleObjectId);
            }

            stream.WriteByte(0);
        }

        private void _writePart2(BitStream stream)
        {
            var hasLevel = Level != 0;

            stream.WriteBit(hasLevel);

            if (hasLevel)
            {
                stream.WriteUInt(Level);
            }
        }

        private void _writePart3(BitStream stream)
        {
            stream.WriteBit(true);
            stream.WriteBit(false);
            stream.WriteBit(true);
        }

        private void _writePart4(BitStream stream, bool creation = false)
        {
            if (creation)
            {
                stream.WriteBit(false);
                stream.WriteBit(false);
                stream.WriteBit(false);
                stream.WriteBit(false);

                stream.WriteUInt((uint) Character.HairColor);
                stream.WriteUInt((uint) Character.HairStyle);
                stream.WriteUInt(0);
                stream.WriteUInt((uint) Character.ShirtColor);
                stream.WriteUInt((uint) Character.PantsColor);
                stream.WriteUInt(0);
                stream.WriteUInt(0);
                stream.WriteUInt((uint) Character.EyebrowStyle);
                stream.WriteUInt((uint) Character.EyeStyle);
                stream.WriteUInt((uint) Character.MouthStyle);
                stream.WriteULong((ulong) Character.User.UserId);
                stream.WriteULong((ulong) Character.LastActivity);
                stream.WriteULong(0);
                stream.WriteULong((ulong) Character.UniverseScore);
                stream.WriteBit(Character.FreeToPlay);

                stream.WriteULong((ulong) Character.TotalCurrencyCollected);
                stream.WriteULong((ulong) Character.TotalBricksCollected);
                stream.WriteULong((ulong) Character.TotalSmashablesSmashed);
                stream.WriteULong((ulong) Character.TotalQuickBuildsCompleted);
                stream.WriteULong((ulong) Character.TotalEnemiesSmashed);
                stream.WriteULong((ulong) Character.TotalRocketsUsed);
                stream.WriteULong((ulong) Character.TotalMissionsCompleted);
                stream.WriteULong((ulong) Character.TotalPetsTamed);
                stream.WriteULong((ulong) Character.TotalImaginationPowerUpsCollected);
                stream.WriteULong((ulong) Character.TotalLifePowerUpsCollected);
                stream.WriteULong((ulong) Character.TotalArmorPowerUpsCollected);
                stream.WriteULong((ulong) Character.TotalDistanceTraveled);
                stream.WriteULong((ulong) Character.TotalSuicides);
                stream.WriteULong((ulong) Character.TotalDamageTaken);
                stream.WriteULong((ulong) Character.TotalDamageHealed);
                stream.WriteULong((ulong) Character.TotalArmorRepaired);
                stream.WriteULong((ulong) Character.TotalImaginationRestored);
                stream.WriteULong((ulong) Character.TotalImaginationUsed);
                stream.WriteULong((ulong) Character.TotalDistanceDriven);
                stream.WriteULong((ulong) Character.TotalTimeAirborne);
                stream.WriteULong((ulong) Character.TotalRacingImaginationPowerUpsCollected);
                stream.WriteULong((ulong) Character.TotalRacingImaginationCratesSmashed);
                stream.WriteULong((ulong) Character.TotalRacecarBoostsActivated);
                stream.WriteULong((ulong) Character.TotalRacecarWrecks);
                stream.WriteULong((ulong) Character.TotalRacingSmashablesSmashed);
                stream.WriteULong((ulong) Character.TotalRacesFinished);
                stream.WriteULong((ulong) Character.TotalFirstPlaceFinishes);
                stream.WriteBit(false);

                var hasRocket = Rocket != null;

                stream.WriteBit(hasRocket);

                if (hasRocket)
                {
                    stream.WriteUShort((ushort) Rocket.Length);
                    stream.WriteString(Rocket, Rocket.Length, true);
                }
            }

            stream.WriteBit(true);
            stream.WriteBit(IsPvP);
            stream.WriteBit(IsGM);
            stream.WriteByte(GMLevel);
            stream.WriteBit(false);
            stream.WriteByte(0);

            stream.WriteBit(true);
            stream.WriteUInt((uint) Activity);

            var hasGuild = GuildId != -1;

            stream.WriteBit(hasGuild);

            if (hasGuild)
            {
                stream.WriteLong(GuildId);
                stream.WriteByte((byte) GuildName.Length);
                stream.WriteString(GuildName, GuildName.Length, true);
                stream.WriteBit(true);
                stream.WriteInt(-1);
            }
        }

        public override void Serialize(BitStream stream)
        {
            _writePart1(stream);
            _writePart2(stream);
            _writePart3(stream);
            _writePart4(stream);
        }

        public override void Construct(BitStream stream)
        {
            _writePart1(stream);
            _writePart2(stream);
            _writePart3(stream);
            _writePart4(stream, true);
        }
    }
}