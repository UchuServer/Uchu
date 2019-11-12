using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("Rewards")]
	public class Rewards
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("LevelID")]
		public int? LevelID { get; set; }

		[Column("MissionID")]
		public int? MissionID { get; set; }

		[Column("RewardType")]
		public int? RewardType { get; set; }

		[Column("value")]
		public int? Value { get; set; }

		[Column("count")]
		public int? Count { get; set; }
	}
}