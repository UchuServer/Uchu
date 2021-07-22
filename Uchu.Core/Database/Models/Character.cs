using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Uchu.Core
{
    [SuppressMessage("ReSharper", "CA2227")]
    public class Character
    {
        [JsonIgnore, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

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
        public long UniverseScore { get; set; }

        [Required]
        public long Currency { get; set; }

        [Required]
        public int MaximumHealth { get; set; } = 4;

        [Required]
        public int CurrentHealth { get; set; } = 4;

        [Required]
        public int BaseHealth { get; set; } = 4;

        [Required]
        public int MaximumArmor { get; set; } = 0;

        [Required]
        public int CurrentArmor { get; set; } = 0;

        [Required]
        public int MaximumImagination { get; set; } = 0;

        [Required]
        public int CurrentImagination { get; set; } = 0;
        
        [Required]
        public int BaseImagination { get; set; }

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

        [Required]
        public bool LandingByRocket { get; set; }
        
        public int InventorySize { get; set; }

        public int VaultInventorySize { get; set; }
        
        public long GuildId { get; set; }

        public int LaunchedRocketFrom { get; set; }
        
        [MaxLength(30)]
        public string Rocket { get; set; }

        [JsonIgnore]
        public long UserId { get; set; }
        
        [JsonIgnore]
        public User User { get; set; }

        public List<InventoryItem> Items { get; set; }
        
        public List<Mission> Missions { get; set; }
        
        public List<UnlockedEmote> UnlockedEmotes { get; set; }
        
        public List<CharacterFlag> Flags { get; set; }

        public float? SpawnPositionX { get; set; }
        
        public float? SpawnPositionY { get; set; }
        
        public float? SpawnPositionZ { get; set; }
        
        public float? SpawnRotationW { get; set; }

        public float? SpawnRotationX { get; set; }
        
        public float? SpawnRotationY { get; set; }
        
        public float? SpawnRotationZ { get; set; }
    }
}