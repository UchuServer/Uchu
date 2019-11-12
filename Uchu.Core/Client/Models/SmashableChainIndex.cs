using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("SmashableChainIndex")]
	public class SmashableChainIndex
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("targetGroup")]
		public string TargetGroup { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("continuous")]
		public int? Continuous { get; set; }
	}
}