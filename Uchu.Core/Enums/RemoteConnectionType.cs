using System.Diagnostics.CodeAnalysis;

namespace Uchu.Core
{
    [SuppressMessage("ReSharper", "CA1028")]
    public enum RemoteConnectionType : ushort
    {
        General,
        Auth,
        Chat,
        Client = 0x04,
        Server
    }
}