using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class RacingHandler : HandlerGroup
    {
        [PacketHandler]
        public void AcknowledgePossessionHandler(AcknowledgePossessionMessage message, Player player)
        {
            var racingControl = player.Zone.ZoneControlObject.GetComponent<RacingControlComponent>();
            racingControl.OnAcknowledgePossession(message);
        }

        [PacketHandler]
        public void VehicleSetWheelLockStateHandler(VehicleSetWheelLockStateMessage message, Player player)
        {
            Logger.Information($"Set wheel lock state: friction = {message.ExtraFriction}, locked = {message.Locked}");
            // TODO: handle
        }
    }
}
