using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("AICombatRoles")]
	public class AICombatRoles
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("preferredRole")]
		public int? PreferredRole { get; set; }

		[Column("specifiedMinRangeNOUSE")]
		public float? SpecifiedMinRangeNOUSE { get; set; }

		[Column("specifiedMaxRangeNOUSE")]
		public float? SpecifiedMaxRangeNOUSE { get; set; }

		[Column("specificMinRange")]
		public float? SpecificMinRange { get; set; }

		[Column("specificMaxRange")]
		public float? SpecificMaxRange { get; set; }
	}
}