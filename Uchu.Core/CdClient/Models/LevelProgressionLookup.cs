using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("LevelProgressionLookup")]
	public class LevelProgressionLookup
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("requiredUScore")]
		public int? RequiredUScore { get; set; }

		[Column("BehaviorEffect")]
		public string BehaviorEffect { get; set; }
	}
}