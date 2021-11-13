using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class RacingHandler : HandlerGroup
    {
        [PacketHandler]
        public void AcknowledgePossessionHandler(AcknowledgePossessionMessage message, Player player)
        {
            message.Associate?.OnAcknowledgePossession.Invoke(message.PossessedObjId);
        }

        [PacketHandler]
        public void VehicleSetWheelLockStateHandler(VehicleSetWheelLockStateMessage message, Player player)
        {
            Logger.Information($"Set wheel lock state: friction = {message.ExtraFriction}, locked = {message.Locked}");
            // TODO: handle
        }

        [PacketHandler]
        public async void ImaginationHandler(VehicleNotifyHitImaginationServerMessage message, Player player)
        {
            message.Associate.GetComponent<DestroyableComponent>().Imagination += 10;
            await message.PickupObjId.GetComponent<DestructibleComponent>().SmashAsync(message.Associate);
        }
    }
}
