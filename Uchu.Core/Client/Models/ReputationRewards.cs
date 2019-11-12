using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ReputationRewards")]
	public class ReputationRewards
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("repLevel")]
		public int? RepLevel { get; set; }

		[Column("sublevel")]
		public int? Sublevel { get; set; }

		[Column("reputation")]
		public float? Reputation { get; set; }
	}
}