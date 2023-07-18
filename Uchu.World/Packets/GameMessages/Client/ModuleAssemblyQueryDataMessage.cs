namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct ModuleAssemblyQueryDataMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.ModuleAssemblyQueryData;
    }
}
