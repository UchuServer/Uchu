using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("ObjectSkills")]
	public class ObjectSkills
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("objectTemplate")]
		public int? ObjectTemplate { get; set; }

		[Column("skillID")]
		public int? SkillID { get; set; }

		[Column("castOnType")]
		public int? CastOnType { get; set; }

		[Column("AICombatWeight")]
		public int? AICombatWeight { get; set; }
	}
}