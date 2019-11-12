using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("RenderIconAssets")]
	public class RenderIconAssets
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("icon_asset")]
		public string Iconasset { get; set; }

		[Column("blank_column")]
		public string Blankcolumn { get; set; }
	}
}