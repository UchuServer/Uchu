using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Uchu.Core
{
    public class MissionTaskValue
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public float Value { get; set; }
        
        [Required]
        public int Count { get; set;}

        public int MissionTaskId { get; set; }

        [ForeignKey(nameof(MissionTaskId))]
        public MissionTask MissionTask { get; set; }

        public float[] ValueArray() => Enumerable.Repeat(Value, Count).ToArray();
    }
}