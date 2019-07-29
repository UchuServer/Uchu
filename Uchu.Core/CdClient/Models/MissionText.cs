using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("MissionText")]
	public class MissionText
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("story_icon")]
		public string Storyicon { get; set; }

		[Column("missionIcon")]
		public string MissionIcon { get; set; }

		[Column("offerNPCIcon")]
		public string OfferNPCIcon { get; set; }

		[Column("IconID")]
		public int? IconID { get; set; }

		[Column("state_1_anim")]
		public string State1anim { get; set; }

		[Column("state_2_anim")]
		public string State2anim { get; set; }

		[Column("state_3_anim")]
		public string State3anim { get; set; }

		[Column("state_4_anim")]
		public string State4anim { get; set; }

		[Column("state_3_turnin_anim")]
		public string State3turninanim { get; set; }

		[Column("state_4_turnin_anim")]
		public string State4turninanim { get; set; }

		[Column("onclick_anim")]
		public string Onclickanim { get; set; }

		[Column("CinematicAccepted")]
		public string CinematicAccepted { get; set; }

		[Column("CinematicAcceptedLeadin")]
		public float? CinematicAcceptedLeadin { get; set; }

		[Column("CinematicCompleted")]
		public string CinematicCompleted { get; set; }

		[Column("CinematicCompletedLeadin")]
		public float? CinematicCompletedLeadin { get; set; }

		[Column("CinematicRepeatable")]
		public string CinematicRepeatable { get; set; }

		[Column("CinematicRepeatableLeadin")]
		public float? CinematicRepeatableLeadin { get; set; }

		[Column("CinematicRepeatableCompleted")]
		public string CinematicRepeatableCompleted { get; set; }

		[Column("CinematicRepeatableCompletedLeadin")]
		public float? CinematicRepeatableCompletedLeadin { get; set; }

		[Column("AudioEventGUID_Interact")]
		public string AudioEventGUIDInteract { get; set; }

		[Column("AudioEventGUID_OfferAccept")]
		public string AudioEventGUIDOfferAccept { get; set; }

		[Column("AudioEventGUID_OfferDeny")]
		public string AudioEventGUIDOfferDeny { get; set; }

		[Column("AudioEventGUID_Completed")]
		public string AudioEventGUIDCompleted { get; set; }

		[Column("AudioEventGUID_TurnIn")]
		public string AudioEventGUIDTurnIn { get; set; }

		[Column("AudioEventGUID_Failed")]
		public string AudioEventGUIDFailed { get; set; }

		[Column("AudioEventGUID_Progress")]
		public string AudioEventGUIDProgress { get; set; }

		[Column("AudioMusicCue_OfferAccept")]
		public string AudioMusicCueOfferAccept { get; set; }

		[Column("AudioMusicCue_TurnIn")]
		public string AudioMusicCueTurnIn { get; set; }

		[Column("turnInIconID")]
		public int? TurnInIconID { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}