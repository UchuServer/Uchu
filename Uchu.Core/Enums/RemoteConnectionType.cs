namespace Uchu.Core
{
    public enum RemoteConnectionType : ushort
    {
        General,
        Auth,
        Chat,
        Server = 4,
        Client
    }
}