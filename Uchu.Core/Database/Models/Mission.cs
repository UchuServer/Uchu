using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Uchu.Core
{
    [SuppressMessage("ReSharper", "CA2227")]
    public class Mission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MissionId { get; set; }

        [Required]
        public int State { get; set; } = (int) MissionState.Active;

        [Required]
        public int CompletionCount { get; set; } = 0;

        [Required]
        public long LastCompletion { get; set; } = 0;

        public long CharacterId { get; set; }
        
        [JsonIgnore]
        public Character Character { get; set; }

        public List<MissionTask> Tasks { get; set; }
    }
}