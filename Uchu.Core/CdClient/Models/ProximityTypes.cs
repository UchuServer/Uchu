using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("ProximityTypes")]
	public class ProximityTypes
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("Name")]
		public string Name { get; set; }

		[Column("Radius")]
		public int? Radius { get; set; }

		[Column("CollisionGroup")]
		public int? CollisionGroup { get; set; }

		[Column("PassiveChecks")]
		public bool? PassiveChecks { get; set; }

		[Column("IconID")]
		public int? IconID { get; set; }

		[Column("LoadOnClient")]
		public bool? LoadOnClient { get; set; }

		[Column("LoadOnServer")]
		public bool? LoadOnServer { get; set; }
	}
}