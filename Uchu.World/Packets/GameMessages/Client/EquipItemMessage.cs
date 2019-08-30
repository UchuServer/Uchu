namespace Uchu.World
{
    public class EquipItemMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0xE7;
        
        public bool IgnoreCooldown { get; set; }
        
        public bool OutSuccess { get; set; }
        
        public long ItemObjectId { get; set; }
    }
}