using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class ActivityScore
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int Activity { get; set; }

        [Required]
        public ushort Zone { get; set; }

        [Required]
        public long CharacterId { get; set; }

        public int Points { get; set; }

        public int Time { get; set; }

        public long LastPlayed { get; set; }

        public int NumPlayed { get; set; }

        // YYYYWW for Weekly leaderboard entries
        // 0 for All-time leaderboard entries
        public int Week { get; set; }
    }
}
