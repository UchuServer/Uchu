using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("PetAbilities")]
	public class PetAbilities
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("AbilityName")]
		public string AbilityName { get; set; }

		[Column("ImaginationCost")]
		public int? ImaginationCost { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }
	}
}