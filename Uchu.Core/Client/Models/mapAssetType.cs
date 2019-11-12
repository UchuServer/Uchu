using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("mapAssetType")]
	public class mapAssetType
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("label")]
		public string Label { get; set; }

		[Column("pathdir")]
		public string Pathdir { get; set; }

		[Column("typelabel")]
		public string Typelabel { get; set; }
	}
}