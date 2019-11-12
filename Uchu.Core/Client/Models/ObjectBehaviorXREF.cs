using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ObjectBehaviorXREF")]
	public class ObjectBehaviorXREF
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("LOT")]
		public int? LOT { get; set; }

		[Column("behaviorID1")]
		public long? BehaviorID1 { get; set; }

		[Column("behaviorID2")]
		public long? BehaviorID2 { get; set; }

		[Column("behaviorID3")]
		public long? BehaviorID3 { get; set; }

		[Column("behaviorID4")]
		public long? BehaviorID4 { get; set; }

		[Column("behaviorID5")]
		public long? BehaviorID5 { get; set; }

		[Column("type")]
		public int? Type { get; set; }
	}
}