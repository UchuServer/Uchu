using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("TextDescription")]
	public class TextDescription
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("TextID")]
		public int? TextID { get; set; }

		[Column("TestDescription")]
		public string TestDescription { get; set; }
	}
}