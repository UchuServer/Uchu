using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class MissionTaskValue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public float Value { get; set; }

        public int MissionTaskId { get; set; }
        public MissionTask MissionTask { get; set; }
    }
}