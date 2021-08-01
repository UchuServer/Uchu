using Uchu.Core;

namespace Uchu.World
{
    public struct VendorSerialization
    {
        public bool HasVendorInfo { get; set; }
        [Requires("HasVendorInfo")]
        public bool HasStandardItems { get; set; }
        [Requires("HasVendorInfo")]
        public bool HasMulticostItems { get; set; }
    }
}