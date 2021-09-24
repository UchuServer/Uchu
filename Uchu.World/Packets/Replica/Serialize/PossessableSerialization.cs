using Uchu.Core;

namespace Uchu.World
{
    public struct PossessableSerialization
    {
        [Default]
        public PossessableInfo PossessableInfo { get; set; }
    }

    [Struct]
    public struct PossessableInfo
    {
        [Default]
        public GameObject Driver { get; set; }
        [Default]
        public uint Animation { get; set; }
        public bool ImmediateDepossess { get; set; }
    }
}
