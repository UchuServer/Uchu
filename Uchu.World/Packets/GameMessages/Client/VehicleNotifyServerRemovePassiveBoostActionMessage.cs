namespace Uchu.World;

[ClientGameMessagePacketStruct]
public struct VehicleNotifyServerRemovePassiveBoostActionMessage
{
    public GameObject Associate { get; set; }
    public GameMessageId GameMessageId => GameMessageId.VehicleNotifyServerRemovePassiveBoostAction;
}
