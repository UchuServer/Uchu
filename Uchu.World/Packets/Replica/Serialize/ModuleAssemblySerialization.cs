using Uchu.Core;

namespace Uchu.World
{
    public struct ModuleAssemblySerialization
    {
        public bool Flag { get; set; }
        [Requires("Flag")]
        public ModuleAssemblyInfo ModuleAssemblyInfo { get; set; }
    }

    [Struct]
    public struct ModuleAssemblyInfo
    {
        [Default]
        public GameObject Assembly { get; set; }
        public bool UseOptionalParts { get; set; }
        [Wide]
        [StoreLengthAs(typeof(ushort))]
        public string Blob { get; set; }
    }
}
