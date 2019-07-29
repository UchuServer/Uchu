using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("PetComponent")]
	public class PetComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("minTameUpdateTime")]
		public float? MinTameUpdateTime { get; set; }

		[Column("maxTameUpdateTime")]
		public float? MaxTameUpdateTime { get; set; }

		[Column("percentTameChance")]
		public float? PercentTameChance { get; set; }

		[Column("tamability")]
		public float? Tamability { get; set; }

		[Column("elementType")]
		public int? ElementType { get; set; }

		[Column("walkSpeed")]
		public float? WalkSpeed { get; set; }

		[Column("runSpeed")]
		public float? RunSpeed { get; set; }

		[Column("sprintSpeed")]
		public float? SprintSpeed { get; set; }

		[Column("idleTimeMin")]
		public float? IdleTimeMin { get; set; }

		[Column("idleTimeMax")]
		public float? IdleTimeMax { get; set; }

		[Column("petForm")]
		public int? PetForm { get; set; }

		[Column("imaginationDrainRate")]
		public float? ImaginationDrainRate { get; set; }

		[Column("AudioMetaEventSet")]
		public string AudioMetaEventSet { get; set; }

		[Column("buffIDs")]
		public string BuffIDs { get; set; }
	}
}