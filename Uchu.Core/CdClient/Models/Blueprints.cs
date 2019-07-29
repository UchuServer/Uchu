using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("Blueprints")]
	public class Blueprints
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public long? Id { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("accountid")]
		public long? Accountid { get; set; }

		[Column("characterid")]
		public long? Characterid { get; set; }

		[Column("price")]
		public int? Price { get; set; }

		[Column("rating")]
		public int? Rating { get; set; }

		[Column("categoryid")]
		public int? Categoryid { get; set; }

		[Column("lxfpath")]
		public string Lxfpath { get; set; }

		[Column("deleted")]
		public bool? Deleted { get; set; }

		[Column("created")]
		public long? Created { get; set; }

		[Column("modified")]
		public long? Modified { get; set; }
	}
}