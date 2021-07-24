using Uchu.Core;

namespace Uchu.World
{
    public struct PossessableSerialization
    {
        public bool PossessableInfoExists { get; set; }
        [Default]
        [Requires("PossessableInfoExists")]
        public GameObject Driver { get; set; }
        [Requires("PossessableInfoExists")]
        [Default]
        public uint Animation { get; set; }
        [Requires("PossessableInfoExists")]
        public bool ImmediateDepossess { get; set; }
    }
}