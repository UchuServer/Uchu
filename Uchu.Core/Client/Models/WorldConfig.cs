using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("WorldConfig")]
	public class WorldConfig
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("WorldConfigID")]
		public int? WorldConfigID { get; set; }

		[Column("pegravityvalue")]
		public float? Pegravityvalue { get; set; }

		[Column("pebroadphaseworldsize")]
		public float? Pebroadphaseworldsize { get; set; }

		[Column("pegameobjscalefactor")]
		public float? Pegameobjscalefactor { get; set; }

		[Column("character_rotation_speed")]
		public float? Characterrotationspeed { get; set; }

		[Column("character_walk_forward_speed")]
		public float? Characterwalkforwardspeed { get; set; }

		[Column("character_walk_backward_speed")]
		public float? Characterwalkbackwardspeed { get; set; }

		[Column("character_walk_strafe_speed")]
		public float? Characterwalkstrafespeed { get; set; }

		[Column("character_walk_strafe_forward_speed")]
		public float? Characterwalkstrafeforwardspeed { get; set; }

		[Column("character_walk_strafe_backward_speed")]
		public float? Characterwalkstrafebackwardspeed { get; set; }

		[Column("character_run_backward_speed")]
		public float? Characterrunbackwardspeed { get; set; }

		[Column("character_run_strafe_speed")]
		public float? Characterrunstrafespeed { get; set; }

		[Column("character_run_strafe_forward_speed")]
		public float? Characterrunstrafeforwardspeed { get; set; }

		[Column("character_run_strafe_backward_speed")]
		public float? Characterrunstrafebackwardspeed { get; set; }

		[Column("global_cooldown")]
		public float? Globalcooldown { get; set; }

		[Column("characterGroundedTime")]
		public float? CharacterGroundedTime { get; set; }

		[Column("characterGroundedSpeed")]
		public float? CharacterGroundedSpeed { get; set; }

		[Column("globalImmunityTime")]
		public float? GlobalImmunityTime { get; set; }

		[Column("character_max_slope")]
		public float? Charactermaxslope { get; set; }

		[Column("defaultrespawntime")]
		public float? Defaultrespawntime { get; set; }

		[Column("mission_tooltip_timeout")]
		public float? Missiontooltiptimeout { get; set; }

		[Column("vendor_buy_multiplier")]
		public float? Vendorbuymultiplier { get; set; }

		[Column("pet_follow_radius")]
		public float? Petfollowradius { get; set; }

		[Column("character_eye_height")]
		public float? Charactereyeheight { get; set; }

		[Column("flight_vertical_velocity")]
		public float? Flightverticalvelocity { get; set; }

		[Column("flight_airspeed")]
		public float? Flightairspeed { get; set; }

		[Column("flight_fuel_ratio")]
		public float? Flightfuelratio { get; set; }

		[Column("flight_max_airspeed")]
		public float? Flightmaxairspeed { get; set; }

		[Column("fReputationPerVote")]
		public float? FReputationPerVote { get; set; }

		[Column("nPropertyCloneLimit")]
		public int? NPropertyCloneLimit { get; set; }

		[Column("defaultHomespaceTemplate")]
		public int? DefaultHomespaceTemplate { get; set; }

		[Column("coins_lost_on_death_percent")]
		public float? Coinslostondeathpercent { get; set; }

		[Column("coins_lost_on_death_min")]
		public int? Coinslostondeathmin { get; set; }

		[Column("coins_lost_on_death_max")]
		public int? Coinslostondeathmax { get; set; }

		[Column("character_votes_per_day")]
		public int? Charactervotesperday { get; set; }

		[Column("property_moderation_request_approval_cost")]
		public int? Propertymoderationrequestapprovalcost { get; set; }

		[Column("property_moderation_request_review_cost")]
		public int? Propertymoderationrequestreviewcost { get; set; }

		[Column("propertyModRequestsAllowedSpike")]
		public int? PropertyModRequestsAllowedSpike { get; set; }

		[Column("propertyModRequestsAllowedInterval")]
		public int? PropertyModRequestsAllowedInterval { get; set; }

		[Column("propertyModRequestsAllowedTotal")]
		public int? PropertyModRequestsAllowedTotal { get; set; }

		[Column("propertyModRequestsSpikeDuration")]
		public int? PropertyModRequestsSpikeDuration { get; set; }

		[Column("propertyModRequestsIntervalDuration")]
		public int? PropertyModRequestsIntervalDuration { get; set; }

		[Column("modelModerateOnCreate")]
		public bool? ModelModerateOnCreate { get; set; }

		[Column("defaultPropertyMaxHeight")]
		public float? DefaultPropertyMaxHeight { get; set; }

		[Column("reputationPerVoteCast")]
		public float? ReputationPerVoteCast { get; set; }

		[Column("reputationPerVoteReceived")]
		public float? ReputationPerVoteReceived { get; set; }

		[Column("showcaseTopModelConsiderationBattles")]
		public int? ShowcaseTopModelConsiderationBattles { get; set; }

		[Column("reputationPerBattlePromotion")]
		public float? ReputationPerBattlePromotion { get; set; }

		[Column("coins_lost_on_death_min_timeout")]
		public float? Coinslostondeathmintimeout { get; set; }

		[Column("coins_lost_on_death_max_timeout")]
		public float? Coinslostondeathmaxtimeout { get; set; }

		[Column("mail_base_fee")]
		public int? Mailbasefee { get; set; }

		[Column("mail_percent_attachment_fee")]
		public float? Mailpercentattachmentfee { get; set; }

		[Column("propertyReputationDelay")]
		public int? PropertyReputationDelay { get; set; }

		[Column("LevelCap")]
		public int? LevelCap { get; set; }

		[Column("LevelUpBehaviorEffect")]
		public string LevelUpBehaviorEffect { get; set; }

		[Column("CharacterVersion")]
		public int? CharacterVersion { get; set; }

		[Column("LevelCapCurrencyConversion")]
		public int? LevelCapCurrencyConversion { get; set; }
	}
}