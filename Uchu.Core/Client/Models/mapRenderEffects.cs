using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("mapRenderEffects")]
	public class mapRenderEffects
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("gameID")]
		public int? GameID { get; set; }

		[Column("description")]
		public string Description { get; set; }
	}
}