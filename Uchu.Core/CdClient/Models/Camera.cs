using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("Camera")]
	public class Camera
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("camera_name")]
		public string Cameraname { get; set; }

		[Column("pitch_angle_tolerance")]
		public float? Pitchangletolerance { get; set; }

		[Column("starting_zoom")]
		public float? Startingzoom { get; set; }

		[Column("zoom_return_modifier")]
		public float? Zoomreturnmodifier { get; set; }

		[Column("pitch_return_modifier")]
		public float? Pitchreturnmodifier { get; set; }

		[Column("tether_out_return_modifier")]
		public float? Tetheroutreturnmodifier { get; set; }

		[Column("tether_in_return_multiplier")]
		public float? Tetherinreturnmultiplier { get; set; }

		[Column("verticle_movement_dampening_modifier")]
		public float? Verticlemovementdampeningmodifier { get; set; }

		[Column("return_from_incline_modifier")]
		public float? Returnfrominclinemodifier { get; set; }

		[Column("horizontal_return_modifier")]
		public float? Horizontalreturnmodifier { get; set; }

		[Column("yaw_behavior_speed_multiplier")]
		public float? Yawbehaviorspeedmultiplier { get; set; }

		[Column("camera_collision_padding")]
		public float? Cameracollisionpadding { get; set; }

		[Column("glide_speed")]
		public float? Glidespeed { get; set; }

		[Column("fade_player_min_range")]
		public float? Fadeplayerminrange { get; set; }

		[Column("min_movement_delta_tolerance")]
		public float? Minmovementdeltatolerance { get; set; }

		[Column("min_glide_distance_tolerance")]
		public float? Minglidedistancetolerance { get; set; }

		[Column("look_forward_offset")]
		public float? Lookforwardoffset { get; set; }

		[Column("look_up_offset")]
		public float? Lookupoffset { get; set; }

		[Column("minimum_vertical_dampening_distance")]
		public float? Minimumverticaldampeningdistance { get; set; }

		[Column("maximum_vertical_dampening_distance")]
		public float? Maximumverticaldampeningdistance { get; set; }

		[Column("minimum_ignore_jump_distance")]
		public float? Minimumignorejumpdistance { get; set; }

		[Column("maximum_ignore_jump_distance")]
		public float? Maximumignorejumpdistance { get; set; }

		[Column("maximum_auto_glide_angle")]
		public float? Maximumautoglideangle { get; set; }

		[Column("minimum_tether_glide_distance")]
		public float? Minimumtetherglidedistance { get; set; }

		[Column("yaw_sign_correction")]
		public float? Yawsigncorrection { get; set; }

		[Column("set_1_look_forward_offset")]
		public float? Set1lookforwardoffset { get; set; }

		[Column("set_1_look_up_offset")]
		public float? Set1lookupoffset { get; set; }

		[Column("set_2_look_forward_offset")]
		public float? Set2lookforwardoffset { get; set; }

		[Column("set_2_look_up_offset")]
		public float? Set2lookupoffset { get; set; }

		[Column("set_0_speed_influence_on_dir")]
		public float? Set0speedinfluenceondir { get; set; }

		[Column("set_1_speed_influence_on_dir")]
		public float? Set1speedinfluenceondir { get; set; }

		[Column("set_2_speed_influence_on_dir")]
		public float? Set2speedinfluenceondir { get; set; }

		[Column("set_0_angular_relaxation")]
		public float? Set0angularrelaxation { get; set; }

		[Column("set_1_angular_relaxation")]
		public float? Set1angularrelaxation { get; set; }

		[Column("set_2_angular_relaxation")]
		public float? Set2angularrelaxation { get; set; }

		[Column("set_0_position_up_offset")]
		public float? Set0positionupoffset { get; set; }

		[Column("set_1_position_up_offset")]
		public float? Set1positionupoffset { get; set; }

		[Column("set_2_position_up_offset")]
		public float? Set2positionupoffset { get; set; }

		[Column("set_0_position_forward_offset")]
		public float? Set0positionforwardoffset { get; set; }

		[Column("set_1_position_forward_offset")]
		public float? Set1positionforwardoffset { get; set; }

		[Column("set_2_position_forward_offset")]
		public float? Set2positionforwardoffset { get; set; }

		[Column("set_0_FOV")]
		public float? Set0FOV { get; set; }

		[Column("set_1_FOV")]
		public float? Set1FOV { get; set; }

		[Column("set_2_FOV")]
		public float? Set2FOV { get; set; }

		[Column("set_0_max_yaw_angle")]
		public float? Set0maxyawangle { get; set; }

		[Column("set_1_max_yaw_angle")]
		public float? Set1maxyawangle { get; set; }

		[Column("set_2_max_yaw_angle")]
		public float? Set2maxyawangle { get; set; }

		[Column("set_1_fade_in_camera_set_change")]
		public int? Set1fadeincamerasetchange { get; set; }

		[Column("set_1_fade_out_camera_set_change")]
		public int? Set1fadeoutcamerasetchange { get; set; }

		[Column("set_2_fade_in_camera_set_change")]
		public int? Set2fadeincamerasetchange { get; set; }

		[Column("set_2_fade_out_camera_set_change")]
		public int? Set2fadeoutcamerasetchange { get; set; }

		[Column("input_movement_scalar")]
		public float? Inputmovementscalar { get; set; }

		[Column("input_rotation_scalar")]
		public float? Inputrotationscalar { get; set; }

		[Column("input_zoom_scalar")]
		public float? Inputzoomscalar { get; set; }

		[Column("minimum_pitch_desired")]
		public float? Minimumpitchdesired { get; set; }

		[Column("maximum_pitch_desired")]
		public float? Maximumpitchdesired { get; set; }

		[Column("minimum_zoom")]
		public float? Minimumzoom { get; set; }

		[Column("maximum_zoom")]
		public float? Maximumzoom { get; set; }

		[Column("horizontal_rotate_tolerance")]
		public float? Horizontalrotatetolerance { get; set; }

		[Column("horizontal_rotate_modifier")]
		public float? Horizontalrotatemodifier { get; set; }
	}
}