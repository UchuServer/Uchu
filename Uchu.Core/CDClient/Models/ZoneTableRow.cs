using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class ZoneTableRow
    {
        [Column("zoneID")]
        public int ZoneId { get; set; }

        [Column("locStatus")]
        public int LocStatus { get; set; }

        [Column("zoneName")]
        public string FileName { get; set; }

        [Column("scriptID")]
        public int ScriptComponentId { get; set; }

        [Column("ghostdistance_min")]
        public float MinimumGhostDistance { get; set; }

        [Column("ghostdistance")]
        public float GhostDistance { get; set; }

        [Column("population_soft_cap")]
        public int SoftPlayerCap { get; set; }

        [Column("population_hard_cap")]
        public int HardPlayerCap { get; set; }

        [Column("DisplayDescription")]
        public string Description { get; set; }

        [Column("mapFolder")]
        public string MapDirectory { get; set; }

        [Column("smashableMinDistance")]
        public float MinimumSmashDistance { get; set; }

        [Column("smashableMaxDistance")]
        public float MaximumSmashDistance { get; set; }

        [Column("mixerProgram")]
        public string MixerProgram { get; set; }

        [Column("clientPhysicsFramerate")]
        public string ClientPhysicsFramerate { get; set; }

        [Column("serverPhysicsFramerate")]
        public string ServerPhysicsFramerate { get; set; }

        [Column("zoneControlTemplate")]
        public int ZoneControlLOT { get; set; }

        [Column("widthInChunks")]
        public int WidthInChunks { get; set; }

        [Column("heightInChunks")]
        public int HeightInChunks { get; set; }

        [Column("petsAllowed")]
        public bool PetsAllowed { get; set; }

        [Column("localize")]
        public bool Localize { get; set; }

        [Column("fZoneWeight")]
        public float ZoneWeight { get; set; }

        [Column("thumbnail")]
        public string Thumbnail { get; set; }

        [Column("PlayerLoseCoinsOnDeath")]
        public bool LoseCoins { get; set; }

        [Column("disableSaveLoc")]
        public bool DisableLocationSave { get; set; }

        [Column("teamRadius")]
        public float TeamRadius { get; set; }

        [Column("gate_version")]
        public string GateVersion { get; set; }

        [Column("mountsAllowed")]
        public bool MountsAllowed { get; set; }
    }
}