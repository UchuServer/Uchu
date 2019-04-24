using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class RebuildComponentRow
    {
        [Column("id")]
        public int RebuildId { get; set; }

        [Column("reset_time")]
        public float ResetTime { get; set; }

        [Column("complete_time")]
        public float CompleteTime { get; set; }

        [Column("take_imagination")]
        public int ImaginationCost { get; set; }

        [Column("interruptible")]
        public bool IsInterruptible { get; set; }

        [Column("self_activator")]
        public bool IsSelfActivated { get; set; }

        [Column("custom_modules")]
        public int[] CustomModules { get; set; }

        [Column("activityID")]
        public int ActivityId { get; set; }

        [Column("post_imagination_cost")]
        public int FinishImaginationCost { get; set; }

        [Column("time_before_smash")]
        public float AliveTime { get; set; }
    }
}