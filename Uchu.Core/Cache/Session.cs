namespace Uchu.Core
{
    public class Session
    {
        public string Key { get; set; }
        public long CharacterId { get; set; } = -1;
        public long UserId { get; set; }
        public int ZoneId { get; set; } = -1;
    }
}