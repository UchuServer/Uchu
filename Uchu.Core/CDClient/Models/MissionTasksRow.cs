using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;

namespace Uchu.Core
{
    public class MissionTasksRow
    {
        [Column("id")]
        public int MissionId { get; set; }

        [Column("locStatus")]
        public int LocStatus { get; set; }

        [Column("taskType")]
        public int TaskType { get; set; }

        [Column("targetGroup")]
        public int[] TargetLOTs { get; set; }

        [Column("targetValue")]
        public int TargetValue { get; set; }

        [Column("taskParam1")]
        public string TaskParameter { get; set; }

        [Column("largeTaskIcon")]
        public string LargeTaskIcon { get; set; }

        [Column("IconID")]
        public int IconId { get; set; }

        [Column("uid")]
        public int UId { get; set; }

        [Column("largeTaskIconID")]
        public int LargeTaskIconId { get; set; }

        [Column("localize")]
        public bool Localize { get; set; }

        [Column("gate_version")]
        public string GateVersion { get; set; }
    }
}