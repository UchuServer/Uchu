using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("Factions")]
	public class Factions
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("faction")]
		public int? Faction { get; set; }

		[Column("factionList")]
		public string FactionList { get; set; }

		[Column("factionListFriendly")]
		public bool? FactionListFriendly { get; set; }

		[Column("friendList")]
		public string FriendList { get; set; }

		[Column("enemyList")]
		public string EnemyList { get; set; }
	}
}