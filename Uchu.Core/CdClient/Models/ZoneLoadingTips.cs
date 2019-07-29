using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("ZoneLoadingTips")]
	public class ZoneLoadingTips
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("zoneid")]
		public int? Zoneid { get; set; }

		[Column("imagelocation")]
		public string Imagelocation { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("weight")]
		public int? Weight { get; set; }

		[Column("targetVersion")]
		public string TargetVersion { get; set; }
	}
}