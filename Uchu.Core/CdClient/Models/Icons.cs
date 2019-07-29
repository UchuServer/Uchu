using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("Icons")]
	public class Icons
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("IconID")]
		public int? IconID { get; set; }

		[Column("IconPath")]
		public string IconPath { get; set; }

		[Column("IconName")]
		public string IconName { get; set; }
	}
}