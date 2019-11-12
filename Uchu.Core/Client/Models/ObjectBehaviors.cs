using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ObjectBehaviors")]
	public class ObjectBehaviors
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("BehaviorID")]
		public long? BehaviorID { get; set; }

		[Column("xmldata")]
		public string Xmldata { get; set; }
	}
}