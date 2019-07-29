using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("ModularBuildComponent")]
	public class ModularBuildComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("buildType")]
		public int? BuildType { get; set; }

		[Column("xml")]
		public string Xml { get; set; }

		[Column("createdLOT")]
		public int? CreatedLOT { get; set; }

		[Column("createdPhysicsID")]
		public int? CreatedPhysicsID { get; set; }

		[Column("AudioEventGUID_Snap")]
		public string AudioEventGUIDSnap { get; set; }

		[Column("AudioEventGUID_Complete")]
		public string AudioEventGUIDComplete { get; set; }

		[Column("AudioEventGUID_Present")]
		public string AudioEventGUIDPresent { get; set; }
	}
}