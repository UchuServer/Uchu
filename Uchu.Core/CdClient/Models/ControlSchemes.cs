using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("ControlSchemes")]
	public class ControlSchemes
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("control_scheme")]
		public int? Controlscheme { get; set; }

		[Column("scheme_name")]
		public string Schemename { get; set; }

		[Column("rotation_speed")]
		public float? Rotationspeed { get; set; }

		[Column("walk_forward_speed")]
		public float? Walkforwardspeed { get; set; }

		[Column("walk_backward_speed")]
		public float? Walkbackwardspeed { get; set; }

		[Column("walk_strafe_speed")]
		public float? Walkstrafespeed { get; set; }

		[Column("walk_strafe_forward_speed")]
		public float? Walkstrafeforwardspeed { get; set; }

		[Column("walk_strafe_backward_speed")]
		public float? Walkstrafebackwardspeed { get; set; }

		[Column("run_backward_speed")]
		public float? Runbackwardspeed { get; set; }

		[Column("run_strafe_speed")]
		public float? Runstrafespeed { get; set; }

		[Column("run_strafe_forward_speed")]
		public float? Runstrafeforwardspeed { get; set; }

		[Column("run_strafe_backward_speed")]
		public float? Runstrafebackwardspeed { get; set; }

		[Column("keyboard_zoom_sensitivity")]
		public float? Keyboardzoomsensitivity { get; set; }

		[Column("keyboard_pitch_sensitivity")]
		public float? Keyboardpitchsensitivity { get; set; }

		[Column("keyboard_yaw_sensitivity")]
		public float? Keyboardyawsensitivity { get; set; }

		[Column("mouse_zoom_wheel_sensitivity")]
		public float? Mousezoomwheelsensitivity { get; set; }

		[Column("x_mouse_move_sensitivity_modifier")]
		public float? Xmousemovesensitivitymodifier { get; set; }

		[Column("y_mouse_move_sensitivity_modifier")]
		public float? Ymousemovesensitivitymodifier { get; set; }

		[Column("freecam_speed_modifier")]
		public float? Freecamspeedmodifier { get; set; }

		[Column("freecam_slow_speed_multiplier")]
		public float? Freecamslowspeedmultiplier { get; set; }

		[Column("freecam_fast_speed_multiplier")]
		public float? Freecamfastspeedmultiplier { get; set; }

		[Column("freecam_mouse_modifier")]
		public float? Freecammousemodifier { get; set; }

		[Column("gamepad_pitch_rot_sensitivity")]
		public float? Gamepadpitchrotsensitivity { get; set; }

		[Column("gamepad_yaw_rot_sensitivity")]
		public float? Gamepadyawrotsensitivity { get; set; }

		[Column("gamepad_trigger_sensitivity")]
		public float? Gamepadtriggersensitivity { get; set; }
	}
}