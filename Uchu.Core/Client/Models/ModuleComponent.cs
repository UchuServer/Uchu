using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ModuleComponent")]
	public class ModuleComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("partCode")]
		public int? PartCode { get; set; }

		[Column("buildType")]
		public int? BuildType { get; set; }

		[Column("xml")]
		public string Xml { get; set; }

		[Column("primarySoundGUID")]
		public string PrimarySoundGUID { get; set; }

		[Column("assembledEffectID")]
		public int? AssembledEffectID { get; set; }
	}
}