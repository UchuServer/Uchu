using RakDotNet;

namespace Uchu.Core.Packets.Server.GameMessages
{
    public class SetJetPackModeMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0231;

        public bool BypassChecks { get; set; } = true;
        
        public bool DoHover { get; set; }
        
        public bool Use { get; set; }

        public int EffectID { get; set; } = -1;

        public float AirSpeed { get; set; } = 10;

        public float MaxAirSpeed { get; set; } = 15;

        public float VerticalSpeed { get; set; } = 1;

        public int WarningEffectID { get; set; } = -1;
        
        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(BypassChecks);
            stream.WriteBit(DoHover);
            stream.WriteBit(Use);
            stream.WriteInt32(EffectID);
            stream.WriteFloat(AirSpeed);
            stream.WriteFloat(MaxAirSpeed);
            stream.WriteFloat(VerticalSpeed);
            stream.WriteInt32(WarningEffectID);
        }
    }
}