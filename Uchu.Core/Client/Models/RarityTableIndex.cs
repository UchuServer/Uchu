using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("RarityTableIndex")]
	public class RarityTableIndex
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("RarityTableIndex")]
		public int? RarityTableIndexColumn { get; set; }
	}
}