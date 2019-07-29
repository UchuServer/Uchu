using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("VehicleStatMap")]
	public class VehicleStatMap
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("ModuleStat")]
		public string ModuleStat { get; set; }

		[Column("HavokStat")]
		public string HavokStat { get; set; }

		[Column("HavokChangePerModuleStat")]
		public float? HavokChangePerModuleStat { get; set; }
	}
}