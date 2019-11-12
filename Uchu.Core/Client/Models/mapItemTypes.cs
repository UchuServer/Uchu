using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("mapItemTypes")]
	public class mapItemTypes
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("equipLocation")]
		public string EquipLocation { get; set; }
	}
}