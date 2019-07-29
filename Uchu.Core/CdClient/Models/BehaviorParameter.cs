using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("BehaviorParameter")]
	public class BehaviorParameter
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("behaviorID")]
		public int? BehaviorID { get; set; }

		[Column("parameterID")]
		public string ParameterID { get; set; }

		[Column("value")]
		public float? Value { get; set; }
	}
}