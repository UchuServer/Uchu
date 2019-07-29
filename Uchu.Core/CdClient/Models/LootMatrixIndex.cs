using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("LootMatrixIndex")]
	public class LootMatrixIndex
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("LootMatrixIndex")]
		public int? LootMatrixIndexColumn { get; set; }

		[Column("inNpcEditor")]
		public bool? InNpcEditor { get; set; }
	}
}