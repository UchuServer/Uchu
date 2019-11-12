using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("FeatureGating")]
	public class FeatureGating
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("featureName")]
		public string FeatureName { get; set; }

		[Column("major")]
		public int? Major { get; set; }

		[Column("current")]
		public int? Current { get; set; }

		[Column("minor")]
		public int? Minor { get; set; }

		[Column("description")]
		public string Description { get; set; }
	}
}