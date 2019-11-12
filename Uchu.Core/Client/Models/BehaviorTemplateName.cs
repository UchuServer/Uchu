using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("BehaviorTemplateName")]
	public class BehaviorTemplateName
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("templateID")]
		public int? TemplateID { get; set; }

		[Column("name")]
		public string Name { get; set; }
	}
}