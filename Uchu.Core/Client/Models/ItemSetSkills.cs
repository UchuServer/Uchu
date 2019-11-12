using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ItemSetSkills")]
	public class ItemSetSkills
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("SkillSetID")]
		public int? SkillSetID { get; set; }

		[Column("SkillID")]
		public int? SkillID { get; set; }

		[Column("SkillCastType")]
		public int? SkillCastType { get; set; }
	}
}