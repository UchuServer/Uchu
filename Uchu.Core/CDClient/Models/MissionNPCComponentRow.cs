using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class MissionNPCComponentRow
    {
        [Column("id")]
        public int LOT { get; set; }

        [Column("missionID")]
        public int MissionId { get; set; }

        [Column("offersMission")]
        public bool OffersMission { get; set; }

        [Column("acceptsMission")]
        public bool AcceptsMission { get; set; }

        [Column("gate_version")]
        public string GateVersion { get; set; }
    }
}