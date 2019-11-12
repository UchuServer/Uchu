using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("RenderComponentWrapper")]
	public class RenderComponentWrapper
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("defaultWrapperAsset")]
		public string DefaultWrapperAsset { get; set; }
	}
}