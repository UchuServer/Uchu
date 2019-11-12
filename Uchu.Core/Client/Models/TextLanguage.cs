using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("TextLanguage")]
	public class TextLanguage
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("TextID")]
		public int? TextID { get; set; }

		[Column("LanguageID")]
		public int? LanguageID { get; set; }

		[Column("Text")]
		public string Text { get; set; }
	}
}