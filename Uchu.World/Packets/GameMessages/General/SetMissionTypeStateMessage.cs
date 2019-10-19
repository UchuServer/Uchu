using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class SetMissionTypeStateMessage : GeneralGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetMissionTypeState;

        public MissionLockState LockState { get; set; } = MissionLockState.New;

        public string SubType { get; set; }

        public string Type { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            var hasState = LockState != MissionLockState.New;

            writer.WriteBit(hasState);

            if (hasState) writer.Write((int) LockState);

            writer.Write((uint) SubType.Length);
            writer.WriteString(SubType, SubType.Length);

            writer.Write((uint) Type.Length);
            writer.WriteString(Type, Type.Length);
        }

        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit())
                LockState = (MissionLockState) reader.Read<int>();

            SubType = reader.ReadString((int) reader.Read<uint>()); 
            
            Type = reader.ReadString((int) reader.Read<uint>());
        }
    }
}