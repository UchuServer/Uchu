using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("mapAnimationPriorities")]
	public class mapAnimationPriorities
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("priority")]
		public float? Priority { get; set; }
	}
}