using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("BehaviorTemplate")]
	public class BehaviorTemplate
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("behaviorID")]
		public int? BehaviorID { get; set; }

		[Column("templateID")]
		public int? TemplateID { get; set; }

		[Column("effectID")]
		public int? EffectID { get; set; }

		[Column("effectHandle")]
		public string EffectHandle { get; set; }
	}
}