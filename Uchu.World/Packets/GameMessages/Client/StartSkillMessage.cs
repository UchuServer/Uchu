using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class StartSkillMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.StartSkill;
        
        public bool UsedMouse { get; set; }
        
        public Item ConsumableItem { get; set; }
        
        public float CasterLatency { get; set; }
        
        public int CastType { get; set; }
        
        public Vector3 LastClickedPosition { get; set; }
        
        public GameObject OptionalOriginator { get; set; }
        
        public GameObject OptionalTarget { get; set; }
        
        public Quaternion OriginatorRotation { get; set; } = Quaternion.Identity;
        
        public byte[] Content { get; set; }
        
        public int SkillId { get; set; }
        
        public uint SkillHandle { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            UsedMouse = reader.ReadBit();

            if (reader.ReadBit())
                ConsumableItem = (Item) reader.ReadGameObject(Associate.Zone);

            if (reader.ReadBit())
                CasterLatency = reader.Read<float>();

            if (reader.ReadBit())
                CastType = reader.Read<int>();

            if (reader.ReadBit())
                LastClickedPosition = reader.Read<Vector3>();

            OptionalOriginator = reader.ReadGameObject(Associate.Zone);

            if (reader.ReadBit())
                OptionalTarget = reader.ReadGameObject(Associate.Zone);

            if (reader.ReadBit())
                OriginatorRotation = reader.Read<Quaternion>();
            
            Content = new byte[reader.Read<uint>()];
            
            for (var i = 0; i < Content.Length; i++)
            {
                Content[i] = reader.Read<byte>();
            }

            SkillId = reader.Read<int>();

            if (reader.ReadBit())
                SkillHandle = reader.Read<uint>();
        }
    }
}