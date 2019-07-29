using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("BaseCombatAIComponent")]
	public class BaseCombatAIComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("behaviorType")]
		public int? BehaviorType { get; set; }

		[Column("combatRoundLength")]
		public float? CombatRoundLength { get; set; }

		[Column("combatRole")]
		public int? CombatRole { get; set; }

		[Column("minRoundLength")]
		public float? MinRoundLength { get; set; }

		[Column("maxRoundLength")]
		public float? MaxRoundLength { get; set; }

		[Column("tetherSpeed")]
		public float? TetherSpeed { get; set; }

		[Column("pursuitSpeed")]
		public float? PursuitSpeed { get; set; }

		[Column("combatStartDelay")]
		public float? CombatStartDelay { get; set; }

		[Column("softTetherRadius")]
		public float? SoftTetherRadius { get; set; }

		[Column("hardTetherRadius")]
		public float? HardTetherRadius { get; set; }

		[Column("spawnTimer")]
		public float? SpawnTimer { get; set; }

		[Column("tetherEffectID")]
		public int? TetherEffectID { get; set; }

		[Column("ignoreMediator")]
		public bool? IgnoreMediator { get; set; }

		[Column("aggroRadius")]
		public float? AggroRadius { get; set; }

		[Column("ignoreStatReset")]
		public bool? IgnoreStatReset { get; set; }

		[Column("ignoreParent")]
		public bool? IgnoreParent { get; set; }
	}
}