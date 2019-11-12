using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("Release_Version")]
	public class ReleaseVersion
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("ReleaseVersion")]
		public string ReleaseVersionColumn { get; set; }

		[Column("ReleaseDate")]
		public long? ReleaseDate { get; set; }
	}
}