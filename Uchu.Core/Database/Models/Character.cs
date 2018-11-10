using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class Character
    {
        public long CharacterId { get; set; }

        [MaxLength(33), Required]
        public string Name { get; set; }

        [MaxLength(33), Required]
        public string CustomName { get; set; }

        [Required]
        public bool NameRejected { get; set; } = false;

        [Required]
        public bool FreeToPlay { get; set; } = false;

        [Required]
        public long ShirtColor { get; set; }

        [Required]
        public long ShirtStyle { get; set; }

        [Required]
        public long PantsColor { get; set; }

        [Required]
        public long HairStyle { get; set; }

        [Required]
        public long HairColor { get; set; }

        [Required]
        public long Lh { get; set; }

        [Required]
        public long Rh { get; set; }

        [Required]
        public long EyebrowStyle { get; set; }

        [Required]
        public long EyeStyle { get; set; }

        [Required]
        public long MouthStyle { get; set; }

        [Required]
        public int LastZone { get; set; }

        [Required]
        public int LastInstance { get; set; }

        [Required]
        public long LastClone { get; set; }

        [Required]
        public long LastActivity { get; set; }

        [Required]
        public long Level { get; set; }

        [Required]
        public long LegoScore { get; set; }

        [Required]
        public long TotalCurrencyCollected { get; set; }

        [Required]
        public long TotalBricksCollected { get; set; }

        [Required]
        public long TotalSmashablesSmashed { get; set; }

        [Required]
        public long TotalQuickBuildsCompleted { get; set; }

        [Required]
        public long TotalEnemiesSmashed { get; set; }

        [Required]
        public long TotalRocketsUsed { get; set; }

        [Required]
        public long TotalMissionsCompleted { get; set; }

        [Required]
        public long TotalPetsTamed { get; set; }

        [Required]
        public long TotalImaginationPowerUpsCollected { get; set; }

        [Required]
        public long TotalLifePowerUpsCollected { get; set; }

        [Required]
        public long TotalArmorPowerUpsCollected { get; set; }

        [Required]
        public long TotalDistanceTraveled { get; set; }

        [Required]
        public long TotalSuicides { get; set; }

        [Required]
        public long TotalDamageTaken { get; set; }

        [Required]
        public long TotalDamageHealed { get; set; }

        [Required]
        public long TotalArmorRepaired { get; set; }

        [Required]
        public long TotalImaginationRestored { get; set; }

        [Required]
        public long TotalImaginationUsed { get; set; }

        [Required]
        public long TotalDistanceDriven { get; set; }

        [Required]
        public long TotalTimeAirborne { get; set; }

        [Required]
        public long TotalRacingImaginationPowerUpsCollected { get; set; }

        [Required]
        public long TotalRacingImaginationCratesSmashed { get; set; }

        [Required]
        public long TotalRacecarBoostsActivated { get; set; }

        [Required]
        public long TotalRacecarWrecks { get; set; }

        [Required]
        public long TotalRacingSmashablesSmashed { get; set; }

        [Required]
        public long TotalRacesFinished { get; set; }

        [Required]
        public long TotalFirstPlaceFinishes { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public List<InventoryItem> Items { get; set; }
    }
}