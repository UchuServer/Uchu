using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("mapTextureResource")]
	public class mapTextureResource
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("texturepath")]
		public string Texturepath { get; set; }

		[Column("SurfaceType")]
		public int? SurfaceType { get; set; }
	}
}