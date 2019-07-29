using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("BrickColors")]
	public class BrickColors
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("red")]
		public float? Red { get; set; }

		[Column("green")]
		public float? Green { get; set; }

		[Column("blue")]
		public float? Blue { get; set; }

		[Column("alpha")]
		public float? Alpha { get; set; }

		[Column("legopaletteid")]
		public int? Legopaletteid { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("validTypes")]
		public int? ValidTypes { get; set; }

		[Column("validCharacters")]
		public int? ValidCharacters { get; set; }

		[Column("factoryValid")]
		public bool? FactoryValid { get; set; }
	}
}