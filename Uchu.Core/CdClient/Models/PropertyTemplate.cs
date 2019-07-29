using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("PropertyTemplate")]
	public class PropertyTemplate
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("mapID")]
		public int? MapID { get; set; }

		[Column("vendorMapID")]
		public int? VendorMapID { get; set; }

		[Column("spawnName")]
		public string SpawnName { get; set; }

		[Column("type")]
		public int? Type { get; set; }

		[Column("sizecode")]
		public int? Sizecode { get; set; }

		[Column("minimumPrice")]
		public int? MinimumPrice { get; set; }

		[Column("rentDuration")]
		public int? RentDuration { get; set; }

		[Column("path")]
		public string Path { get; set; }

		[Column("cloneLimit")]
		public int? CloneLimit { get; set; }

		[Column("durationType")]
		public int? DurationType { get; set; }

		[Column("achievementRequired")]
		public int? AchievementRequired { get; set; }

		[Column("zoneX")]
		public float? ZoneX { get; set; }

		[Column("zoneY")]
		public float? ZoneY { get; set; }

		[Column("zoneZ")]
		public float? ZoneZ { get; set; }

		[Column("maxBuildHeight")]
		public float? MaxBuildHeight { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("reputationPerMinute")]
		public int? ReputationPerMinute { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}