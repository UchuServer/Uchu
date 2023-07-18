using Uchu.Core;

namespace Uchu.World
{
    [Struct]
    public struct PreRacePlayerInfo {
        public GameObject Player { get; set; }
        public GameObject Vehicle { get; set; }
        public uint StartingPosition { get; set; }
        public bool IsReady { get; set; }
    }

    [Struct]
    public struct PostRacePlayerInfo {
        public GameObject Player { get; set; }
        public uint CurrentRank { get; set; }
    }

    [Struct]
    public struct DuringRacePlayerInfo {
        public GameObject Player { get; set; }
        public float BestLapTime { get; set; }
        public float RaceTime { get; set; }
    }

    [Struct]
    public struct RaceInfo {
        public ushort LapCount { get; set; }
        [Wide]
        [StoreLengthAs(typeof(ushort))]
        public string PathName { get; set; }
    }

    public struct RacingControlSerialization
    {
        [Default]
        public ushort ExpectedPlayerCount { get; set; }
        [Default]
        [NoLength]
        public PreRacePlayerInfo[] PreRacePlayerInfos { get; set; }
        [Default]
        [NoLength]
        public PostRacePlayerInfo[] PostRacePlayerInfos { get; set; }
        [Default]
        public RaceInfo RaceInfo { get; set; } // this is still good, at least the LapCount
        [Default]
        [NoLength]
        public DuringRacePlayerInfo[] DuringRacePlayerInfos { get; set; }
    }
}
