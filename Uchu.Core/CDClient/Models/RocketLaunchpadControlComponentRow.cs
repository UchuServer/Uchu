using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class RocketLaunchpadControlComponentRow
    {
        [Column("id")]
        public int LaunchpadId { get; set; }

        [Column("targetZone")]
        public int TargetZone { get; set; }

        [Column("defaultZoneID")]
        public int DefaultZoneId { get; set; }

        [Column("targetScene")]
        public string TargetScene { get; set; }

        [Column("gmLevel")]
        public int GmLevel { get; set; }

        [Column("playerAnimation")]
        public string PlayerAnimation { get; set; }

        [Column("rocketAnimation")]
        public string RocketAnimation { get; set; }

        [Column("launchMusic")]
        public string LaunchMusic { get; set; }

        [Column("useLaunchPrecondition")]
        public bool UseLaunchPrecondition { get; set; }

        [Column("useAltLandingPrecondition")]
        public bool UseLandingPrecondition { get; set; }

        [Column("launchPrecondition")]
        public string LaunchPrecondition { get; set; }

        [Column("altLandingPrecondition")]
        public string LandingPrecondition { get; set; }

        [Column("altLandingSpawnPointName")]
        public string LandingSpawnpointName { get; set; }
    }
}