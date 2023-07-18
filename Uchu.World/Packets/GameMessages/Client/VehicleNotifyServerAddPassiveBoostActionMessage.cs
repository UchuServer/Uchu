namespace Uchu.World;

[ClientGameMessagePacketStruct]
public struct VehicleNotifyServerAddPassiveBoostActionMessage
{
    public GameObject Associate { get; set; }
    public GameMessageId GameMessageId => GameMessageId.VehicleNotifyServerAddPassiveBoostAction;
}
