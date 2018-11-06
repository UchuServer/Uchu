namespace Uchu.Core
{
    public enum RemoteConnectionType : ushort
    {
        General,
        Auth,
        Chat,
        Client = 0x04,
        Server
    }
}