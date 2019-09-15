using System;
using RakDotNet.IO;

namespace Uchu.World
{
    public class SetJetPackModeMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetJetPackMode;

        public bool BypassChecks { get; set; }

        public bool DoHover { get; set; }

        public bool Use { get; set; }

        public int EffectId { get; set; } = -1;

        public float AirSpeed { get; set; } = 10;

        public float MaxAirSpeed { get; set; } = 15;

        public float VerticalVelocity { get; set; } = 1;

        public int WarningEffectId { get; set; } = -1;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(BypassChecks);
            writer.WriteBit(DoHover);
            writer.WriteBit(Use);

            var hasEffectId = EffectId != -1;
            writer.WriteBit(hasEffectId);
            if (hasEffectId) writer.Write(EffectId);

            var hasAirSpeed = Math.Abs(AirSpeed - 10) > 0.01f;
            writer.WriteBit(hasAirSpeed);
            if (hasAirSpeed) writer.Write(AirSpeed);

            var hasMaxAirSpeed = Math.Abs(MaxAirSpeed - 15) > 0.01f;
            writer.WriteBit(hasMaxAirSpeed);
            if (hasMaxAirSpeed) writer.Write(MaxAirSpeed);

            var hasVerticalVelocity = Math.Abs(VerticalVelocity - 1) > 0.01f;
            writer.WriteBit(hasVerticalVelocity);
            if (hasVerticalVelocity) writer.Write(VerticalVelocity);

            var hasWarningEffectId = WarningEffectId != -1;
            writer.WriteBit(hasWarningEffectId);
            if (hasWarningEffectId) writer.Write(WarningEffectId);
        }
    }
}