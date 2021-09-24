using Uchu.Core;

namespace Uchu.World
{
    public struct ModuleAssemblySerialization
    {
        [Default]
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
