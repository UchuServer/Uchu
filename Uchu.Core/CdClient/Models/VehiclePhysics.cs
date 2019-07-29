using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("VehiclePhysics")]
	public class VehiclePhysics
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("hkxFilename")]
		public string HkxFilename { get; set; }

		[Column("fGravityScale")]
		public float? FGravityScale { get; set; }

		[Column("fMass")]
		public float? FMass { get; set; }

		[Column("fChassisFriction")]
		public float? FChassisFriction { get; set; }

		[Column("fMaxSpeed")]
		public float? FMaxSpeed { get; set; }

		[Column("fEngineTorque")]
		public float? FEngineTorque { get; set; }

		[Column("fBrakeFrontTorque")]
		public float? FBrakeFrontTorque { get; set; }

		[Column("fBrakeRearTorque")]
		public float? FBrakeRearTorque { get; set; }

		[Column("fBrakeMinInputToBlock")]
		public float? FBrakeMinInputToBlock { get; set; }

		[Column("fBrakeMinTimeToBlock")]
		public float? FBrakeMinTimeToBlock { get; set; }

		[Column("fSteeringMaxAngle")]
		public float? FSteeringMaxAngle { get; set; }

		[Column("fSteeringSpeedLimitForMaxAngle")]
		public float? FSteeringSpeedLimitForMaxAngle { get; set; }

		[Column("fSteeringMinAngle")]
		public float? FSteeringMinAngle { get; set; }

		[Column("fFwdBias")]
		public float? FFwdBias { get; set; }

		[Column("fFrontTireFriction")]
		public float? FFrontTireFriction { get; set; }

		[Column("fRearTireFriction")]
		public float? FRearTireFriction { get; set; }

		[Column("fFrontTireFrictionSlide")]
		public float? FFrontTireFrictionSlide { get; set; }

		[Column("fRearTireFrictionSlide")]
		public float? FRearTireFrictionSlide { get; set; }

		[Column("fFrontTireSlipAngle")]
		public float? FFrontTireSlipAngle { get; set; }

		[Column("fRearTireSlipAngle")]
		public float? FRearTireSlipAngle { get; set; }

		[Column("fWheelWidth")]
		public float? FWheelWidth { get; set; }

		[Column("fWheelRadius")]
		public float? FWheelRadius { get; set; }

		[Column("fWheelMass")]
		public float? FWheelMass { get; set; }

		[Column("fReorientPitchStrength")]
		public float? FReorientPitchStrength { get; set; }

		[Column("fReorientRollStrength")]
		public float? FReorientRollStrength { get; set; }

		[Column("fSuspensionLength")]
		public float? FSuspensionLength { get; set; }

		[Column("fSuspensionStrength")]
		public float? FSuspensionStrength { get; set; }

		[Column("fSuspensionDampingCompression")]
		public float? FSuspensionDampingCompression { get; set; }

		[Column("fSuspensionDampingRelaxation")]
		public float? FSuspensionDampingRelaxation { get; set; }

		[Column("iChassisCollisionGroup")]
		public int? IChassisCollisionGroup { get; set; }

		[Column("fNormalSpinDamping")]
		public float? FNormalSpinDamping { get; set; }

		[Column("fCollisionSpinDamping")]
		public float? FCollisionSpinDamping { get; set; }

		[Column("fCollisionThreshold")]
		public float? FCollisionThreshold { get; set; }

		[Column("fTorqueRollFactor")]
		public float? FTorqueRollFactor { get; set; }

		[Column("fTorquePitchFactor")]
		public float? FTorquePitchFactor { get; set; }

		[Column("fTorqueYawFactor")]
		public float? FTorqueYawFactor { get; set; }

		[Column("fInertiaRoll")]
		public float? FInertiaRoll { get; set; }

		[Column("fInertiaPitch")]
		public float? FInertiaPitch { get; set; }

		[Column("fInertiaYaw")]
		public float? FInertiaYaw { get; set; }

		[Column("fExtraTorqueFactor")]
		public float? FExtraTorqueFactor { get; set; }

		[Column("fCenterOfMassFwd")]
		public float? FCenterOfMassFwd { get; set; }

		[Column("fCenterOfMassUp")]
		public float? FCenterOfMassUp { get; set; }

		[Column("fCenterOfMassRight")]
		public float? FCenterOfMassRight { get; set; }

		[Column("fWheelHardpointFrontFwd")]
		public float? FWheelHardpointFrontFwd { get; set; }

		[Column("fWheelHardpointFrontUp")]
		public float? FWheelHardpointFrontUp { get; set; }

		[Column("fWheelHardpointFrontRight")]
		public float? FWheelHardpointFrontRight { get; set; }

		[Column("fWheelHardpointRearFwd")]
		public float? FWheelHardpointRearFwd { get; set; }

		[Column("fWheelHardpointRearUp")]
		public float? FWheelHardpointRearUp { get; set; }

		[Column("fWheelHardpointRearRight")]
		public float? FWheelHardpointRearRight { get; set; }

		[Column("fInputTurnSpeed")]
		public float? FInputTurnSpeed { get; set; }

		[Column("fInputDeadTurnBackSpeed")]
		public float? FInputDeadTurnBackSpeed { get; set; }

		[Column("fInputAccelSpeed")]
		public float? FInputAccelSpeed { get; set; }

		[Column("fInputDeadAccelDownSpeed")]
		public float? FInputDeadAccelDownSpeed { get; set; }

		[Column("fInputDecelSpeed")]
		public float? FInputDecelSpeed { get; set; }

		[Column("fInputDeadDecelDownSpeed")]
		public float? FInputDeadDecelDownSpeed { get; set; }

		[Column("fInputSlopeChangePointX")]
		public float? FInputSlopeChangePointX { get; set; }

		[Column("fInputInitialSlope")]
		public float? FInputInitialSlope { get; set; }

		[Column("fInputDeadZone")]
		public float? FInputDeadZone { get; set; }

		[Column("fAeroAirDensity")]
		public float? FAeroAirDensity { get; set; }

		[Column("fAeroFrontalArea")]
		public float? FAeroFrontalArea { get; set; }

		[Column("fAeroDragCoefficient")]
		public float? FAeroDragCoefficient { get; set; }

		[Column("fAeroLiftCoefficient")]
		public float? FAeroLiftCoefficient { get; set; }

		[Column("fAeroExtraGravity")]
		public float? FAeroExtraGravity { get; set; }

		[Column("fBoostTopSpeed")]
		public float? FBoostTopSpeed { get; set; }

		[Column("fBoostCostPerSecond")]
		public float? FBoostCostPerSecond { get; set; }

		[Column("fBoostAccelerateChange")]
		public float? FBoostAccelerateChange { get; set; }

		[Column("fBoostDampingChange")]
		public float? FBoostDampingChange { get; set; }

		[Column("fPowerslideNeutralAngle")]
		public float? FPowerslideNeutralAngle { get; set; }

		[Column("fPowerslideTorqueStrength")]
		public float? FPowerslideTorqueStrength { get; set; }

		[Column("iPowerslideNumTorqueApplications")]
		public int? IPowerslideNumTorqueApplications { get; set; }

		[Column("fImaginationTankSize")]
		public float? FImaginationTankSize { get; set; }

		[Column("fSkillCost")]
		public float? FSkillCost { get; set; }

		[Column("fWreckSpeedBase")]
		public float? FWreckSpeedBase { get; set; }

		[Column("fWreckSpeedPercent")]
		public float? FWreckSpeedPercent { get; set; }

		[Column("fWreckMinAngle")]
		public float? FWreckMinAngle { get; set; }

		[Column("AudioEventEngine")]
		public string AudioEventEngine { get; set; }

		[Column("AudioEventSkid")]
		public string AudioEventSkid { get; set; }

		[Column("AudioEventLightHit")]
		public string AudioEventLightHit { get; set; }

		[Column("AudioSpeedThresholdLightHit")]
		public float? AudioSpeedThresholdLightHit { get; set; }

		[Column("AudioTimeoutLightHit")]
		public float? AudioTimeoutLightHit { get; set; }

		[Column("AudioEventHeavyHit")]
		public string AudioEventHeavyHit { get; set; }

		[Column("AudioSpeedThresholdHeavyHit")]
		public float? AudioSpeedThresholdHeavyHit { get; set; }

		[Column("AudioTimeoutHeavyHit")]
		public float? AudioTimeoutHeavyHit { get; set; }

		[Column("AudioEventStart")]
		public string AudioEventStart { get; set; }

		[Column("AudioEventTreadConcrete")]
		public string AudioEventTreadConcrete { get; set; }

		[Column("AudioEventTreadSand")]
		public string AudioEventTreadSand { get; set; }

		[Column("AudioEventTreadWood")]
		public string AudioEventTreadWood { get; set; }

		[Column("AudioEventTreadDirt")]
		public string AudioEventTreadDirt { get; set; }

		[Column("AudioEventTreadPlastic")]
		public string AudioEventTreadPlastic { get; set; }

		[Column("AudioEventTreadGrass")]
		public string AudioEventTreadGrass { get; set; }

		[Column("AudioEventTreadGravel")]
		public string AudioEventTreadGravel { get; set; }

		[Column("AudioEventTreadMud")]
		public string AudioEventTreadMud { get; set; }

		[Column("AudioEventTreadWater")]
		public string AudioEventTreadWater { get; set; }

		[Column("AudioEventTreadSnow")]
		public string AudioEventTreadSnow { get; set; }

		[Column("AudioEventTreadIce")]
		public string AudioEventTreadIce { get; set; }

		[Column("AudioEventTreadMetal")]
		public string AudioEventTreadMetal { get; set; }

		[Column("AudioEventTreadLeaves")]
		public string AudioEventTreadLeaves { get; set; }

		[Column("AudioEventLightLand")]
		public string AudioEventLightLand { get; set; }

		[Column("AudioAirtimeForLightLand")]
		public float? AudioAirtimeForLightLand { get; set; }

		[Column("AudioEventHeavyLand")]
		public string AudioEventHeavyLand { get; set; }

		[Column("AudioAirtimeForHeavyLand")]
		public float? AudioAirtimeForHeavyLand { get; set; }

		[Column("bWheelsVisible")]
		public bool? BWheelsVisible { get; set; }
	}
}