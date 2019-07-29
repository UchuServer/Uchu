using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("BuffParameters")]
	public class BuffParameters
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("BuffID")]
		public int? BuffID { get; set; }

		[Column("ParameterName")]
		public string ParameterName { get; set; }

		[Column("NumberValue")]
		public float? NumberValue { get; set; }

		[Column("StringValue")]
		public string StringValue { get; set; }

		[Column("EffectID")]
		public int? EffectID { get; set; }
	}
}