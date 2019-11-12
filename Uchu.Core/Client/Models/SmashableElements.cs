using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("SmashableElements")]
	public class SmashableElements
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("elementID")]
		public int? ElementID { get; set; }

		[Column("dropWeight")]
		public int? DropWeight { get; set; }
	}
}