using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class ModularBuildingHandler : HandlerGroup
    {
        [PacketHandler]
        public void StartBuildingHandler(StartBuildingWithItemMessage message, Player player)
        {
            player.GetComponent<ModularBuilder>().StartBuilding(message);
        }

        [PacketHandler]
        public void ModularBuildFinishHandler(ModularBuildFinishMessage message, Player player)
        {
            player.GetComponent<ModularBuilder>().FinishBuilding(message.Modules);
        }

        [PacketHandler]
        public void DoneArrangingHandler(DoneArrangingWithItemMessage message, Player player)
        {
            player.GetComponent<ModularBuilder>().DoneArranging(message);
        }

        [PacketHandler]
        public void PickupModelHandler(ModularBuildMoveAndEquipMessage message, Player player)
        {
            player.GetComponent<ModularBuilder>().Pickup(message.Lot);
        }
    }
}