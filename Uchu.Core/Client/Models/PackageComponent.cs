using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("PackageComponent")]
	public class PackageComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("LootMatrixIndex")]
		public int? LootMatrixIndex { get; set; }

		[Column("packageType")]
		public int? PackageType { get; set; }
	}
}