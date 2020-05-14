using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class SetBuildModeMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetBuildMode;

        public bool Start { get; set; }

        public int DistanceType { get; set; } = -1;
        
        public bool ModePaused { get; set; }
        
        public int ModeValue { get; set; }
        
        public Player Player { get; set; }
        
        public Vector3 StartPosition { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Start = reader.ReadBit();

            if (reader.ReadBit())
            {
                DistanceType = reader.Read<int>();
            }

            ModePaused = reader.ReadBit();

            ModeValue = reader.Read<int>();

            Player = reader.ReadGameObject<Player>(Associate.Zone);

            if (reader.ReadBit())
            {
                StartPosition = reader.Read<Vector3>();
            }
        }
    }
}