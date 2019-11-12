using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("DevModelBehaviors")]
	public class DevModelBehaviors
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("ModelID")]
		public int? ModelID { get; set; }

		[Column("BehaviorID")]
		public int? BehaviorID { get; set; }
	}
}