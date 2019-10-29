using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class ModularBuildingHandler : HandlerGroup
    {
        [PacketHandler]
        public void StartBuildingHandler(StartBuildingWithItemMessage message, Player player)
        {
            player.GetComponent<ModularBuilderComponent>().StartBuilding(message);
        }

        [PacketHandler]
        public void ModularBuildFinishHandler(ModularBuildFinishMessage message, Player player)
        {
            player.GetComponent<ModularBuilderComponent>().FinishBuilding(message.Modules);
        }

        [PacketHandler]
        public void DoneArrangingHandler(DoneArrangingWithItemMessage message, Player player)
        {
            player.GetComponent<ModularBuilderComponent>().DoneArranging(message);
        }

        [PacketHandler]
        public void PickupModelHandler(ModularBuildMoveAndEquipMessage message, Player player)
        {
            player.GetComponent<ModularBuilderComponent>().Pickup(message.Lot);
        }

        [PacketHandler]
        public void ModelPrefabHandler(StartArrangingWithModelMessage message, Player player)
        {
            
        }
    }
}