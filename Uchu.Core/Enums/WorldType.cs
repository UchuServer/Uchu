using System.Diagnostics.CodeAnalysis;

namespace Uchu.World
{
    [SuppressMessage("ReSharper", "CA1028")]
    public enum WorldType : uint
    {
        Normal,
        Activity = 4
    }
}