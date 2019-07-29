using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("LanguageType")]
	public class LanguageType
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("LanguageID")]
		public int? LanguageID { get; set; }

		[Column("LanguageDescription")]
		public string LanguageDescription { get; set; }
	}
}