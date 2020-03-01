using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        public async Task ModularBuildFinishHandler(ModularBuildFinishMessage message, Player player)
        {
            await player.GetComponent<ModularBuilderComponent>().FinishBuilding(message.Modules);
        }

        [PacketHandler]
        public void DoneArrangingHandler(DoneArrangingWithItemMessage message, Player player)
        {
            player.GetComponent<ModularBuilderComponent>().DoneArranging(message);
        }

        [PacketHandler]
        public async Task PickupModelHandler(ModularBuildMoveAndEquipMessage message, Player player)
        {
            await player.GetComponent<ModularBuilderComponent>().Pickup(message.Lot);
        }

        [PacketHandler]
        public void ModelPrefabHandler(StartArrangingWithModelMessage message, Player player)
        {
            
        }

        [PacketHandler]
        public async Task BuildExitConfirmationHandler(BuildExitConfirmationMessage message, Player player)
        {
            await player.GetComponent<ModularBuilderComponent>().ConfirmFinish();
        }

        [PacketHandler]
        public async Task SetLastCustomBuildHandler(SetLastCustomBuildMessage message, Player player)
        {
            await using var ctx = new UchuContext();

            var character = await ctx.Characters.FirstAsync(c => c.CharacterId == player.ObjectId);

            character.Rocket = message.Tokens;

            await ctx.SaveChangesAsync();
        }
    }
}