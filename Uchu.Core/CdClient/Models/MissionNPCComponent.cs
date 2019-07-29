using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("MissionNPCComponent")]
	public class MissionNPCComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("missionID")]
		public int? MissionID { get; set; }

		[Column("offersMission")]
		public bool? OffersMission { get; set; }

		[Column("acceptsMission")]
		public bool? AcceptsMission { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}