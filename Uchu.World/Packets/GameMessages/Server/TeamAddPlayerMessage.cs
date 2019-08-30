using RakDotNet;
using RakDotNet.IO;

namespace Uchu.World
{
    public class TeamAddPlayerMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x617;
        
        public bool IsFreeTrail { get; set; }
        
        public bool IsLocal { get; set; }
        
        public bool NoLootOnDeath { get; set; }
        
        public Player Player { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            if (writer.Flag(IsFreeTrail))
            {
                writer.WriteBit(IsFreeTrail);
            }

            if (writer.Flag(IsLocal))
            {
                writer.WriteBit(IsLocal);
            }

            if (writer.Flag(NoLootOnDeath))
            {
                writer.WriteBit(NoLootOnDeath);
            }

            writer.Write(Player.ObjectId);

            writer.WriteString(Player.Name, wide: true);

            writer.WriteBit(false);
            
            if (writer.Flag(Player.Zone.ZoneInfo.ZoneId != 0))
            {
                writer.Write((ushort) Player.Zone.ZoneInfo.ZoneId);
            }
        }
    }
}