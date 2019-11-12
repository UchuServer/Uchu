using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ProximityMonitorComponent")]
	public class ProximityMonitorComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("Proximities")]
		public string Proximities { get; set; }

		[Column("LoadOnClient")]
		public bool? LoadOnClient { get; set; }

		[Column("LoadOnServer")]
		public bool? LoadOnServer { get; set; }
	}
}