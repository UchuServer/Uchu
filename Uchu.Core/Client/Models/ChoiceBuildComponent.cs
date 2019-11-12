using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ChoiceBuildComponent")]
	public class ChoiceBuildComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("selections")]
		public string Selections { get; set; }

		[Column("imaginationOverride")]
		public int? ImaginationOverride { get; set; }
	}
}