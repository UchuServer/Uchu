using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("SurfaceType")]
	public class SurfaceType
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("SurfaceType")]
		public int? SurfaceTypeColumn { get; set; }

		[Column("FootstepNDAudioMetaEventSetName")]
		public string FootstepNDAudioMetaEventSetName { get; set; }
	}
}