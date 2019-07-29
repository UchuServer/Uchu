using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("PropertyEntranceComponent")]
	public class PropertyEntranceComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("mapID")]
		public int? MapID { get; set; }

		[Column("propertyName")]
		public string PropertyName { get; set; }

		[Column("isOnProperty")]
		public bool? IsOnProperty { get; set; }

		[Column("groupType")]
		public string GroupType { get; set; }
	}
}