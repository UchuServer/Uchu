using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("AnimationIndex")]
	public class AnimationIndex
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("animationGroupID")]
		public int? AnimationGroupID { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("groupType")]
		public string GroupType { get; set; }
	}
}