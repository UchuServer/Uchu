using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("DeletionRestrictions")]
	public class DeletionRestrictions
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("restricted")]
		public bool? Restricted { get; set; }

		[Column("ids")]
		public string Ids { get; set; }

		[Column("checkType")]
		public int? CheckType { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}