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
            if (message.Associate is null || message.PickupObjId is null)
                return;
            if (message.Associate.TryGetComponent<DestroyableComponent>(out var vehicleDestroyableComponent))
                vehicleDestroyableComponent.Imagination += 10;
            if (message.PickupObjId.TryGetComponent<DestructibleComponent>(out var imaginationDestructibleComponent))
                await imaginationDestructibleComponent.SmashAsync(message.Associate);

            // Progress imagination collect task
            if (message.Associate.TryGetComponent<PossessableComponent>(out PossessableComponent possessableComponent))
                if (possessableComponent.Driver.TryGetComponent<MissionInventoryComponent>(out MissionInventoryComponent missionInventoryComponent))
                    await missionInventoryComponent.RacingCollectImaginationAsync(player.Zone.ZoneId);
        }

        [PacketHandler]
        public async void RacingPlayerInfoResetFinishedHandler(RacingPlayerInfoResetFinishedMessage message, Player player)
        {
            await player.OnRacingPlayerInfoResetFinished.InvokeAsync();
        }

        [PacketHandler]
        public void VehicleNotifyServerAddPassiveBoostActionHandler(VehicleNotifyServerAddPassiveBoostActionMessage message, Player player)
        {
            player.Zone.ExcludingMessage(new VehicleAddPassiveBoostAction
            {
                Associate = message.Associate,
            }, player);
        }

        [PacketHandler]
        public void VehicleNotifyServerRemovePassiveBoostActionHandler(VehicleNotifyServerRemovePassiveBoostActionMessage message, Player player)
        {
            player.Zone.ExcludingMessage(new VehicleRemovePassiveBoostAction
            {
                Associate = message.Associate,
            }, player);
        }
    }
}
