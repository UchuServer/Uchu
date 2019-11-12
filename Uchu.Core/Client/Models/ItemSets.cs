using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ItemSets")]
	public class ItemSets
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("setID")]
		public int? SetID { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("itemIDs")]
		public string ItemIDs { get; set; }

		[Column("kitType")]
		public int? KitType { get; set; }

		[Column("kitRank")]
		public int? KitRank { get; set; }

		[Column("kitImage")]
		public int? KitImage { get; set; }

		[Column("skillSetWith2")]
		public int? SkillSetWith2 { get; set; }

		[Column("skillSetWith3")]
		public int? SkillSetWith3 { get; set; }

		[Column("skillSetWith4")]
		public int? SkillSetWith4 { get; set; }

		[Column("skillSetWith5")]
		public int? SkillSetWith5 { get; set; }

		[Column("skillSetWith6")]
		public int? SkillSetWith6 { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }

		[Column("kitID")]
		public int? KitID { get; set; }

		[Column("priority")]
		public float? Priority { get; set; }
	}
}