using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class ModularBuildingHandler : HandlerGroup
    {
        // These Handlers are in the order in which the packets come in. Also see BuildBorderComponent

        // Is sent when the player interacts with the build border
        [PacketHandler]
        public void StartBuildingWithItemHandler(StartBuildingWithItemMessage message, Player player)
        {
            Logger.Debug("[MOD_BUILD] StartBuildingWithItem");
            player.GetComponent<ModularBuilderComponent>().StartBuild(message);
        }

        // Is sent directly after StartBuildingWithItem
        [PacketHandler]
        public void SetBuildModeHandler(SetBuildModeMessage message, Player player)
        {
            Logger.Debug("[MOD_BUILD] SetBuildMode");
            player.GetComponent<ModularBuilderComponent>().SetBuildMode(message);
        }

        // Is sent directly after SetBuildMode to acknowledge that the build mode was set
        [PacketHandler]
        public void BuildModeSetHandler(BuildModeSetMessage message, Player player)
        {
            Logger.Debug("[MOD_BUILD] BuildModeSet");
            Logger.Debug($"Build mode set to {message.ModeValue}");
        }

        // Is sent after putting a model into the blueprint
        [PacketHandler]
        public async Task StartArrangingWithModelHandler(StartArrangingWithModelMessage message, Player player)
        {
            Logger.Debug("[MOD_BUILD] ModelPrefab");
            await player.GetComponent<ModularBuilderComponent>().ModelAdded(message);
        }

        // Is sent after picking up a part from the blueprint or equiping a part
        [PacketHandler]
        public void ModularBuildMoveAndEquipHandler(ModularBuildMoveAndEquipMessage message, Player player)
        {
            Logger.Debug("[MOD_BUILD] ModelPrefab");
            player.GetComponent<ModularBuilderComponent>().MoveAndEquip(message);
        }

        // Is sent if the player finishes the build
        [PacketHandler]
        public async Task ModularBuildFinishHandler(ModularBuildFinishMessage message, Player player)
        {
            Logger.Debug("[MOD_BUILD] ModularBuildFinish");
            await player.GetComponent<ModularBuilderComponent>().FinishBuild(message);
        }

        // Is sent after the player has exited the build
        [PacketHandler]
        public void BuildExitConfirmationHandler(BuildExitConfirmationMessage message, Player player)
        {
            Logger.Debug("[MOD_BUILD] BuildExitConfirmation");
            player.GetComponent<ModularBuilderComponent>().ConfirmExitBuild(message);
        }

        // Unknown
        [PacketHandler]
        public void DoneArrangingHandler(DoneArrangingWithItemMessage message, Player player)
        {
            Logger.Debug("[MOD_BUILD] DoneArranging");
        }
    }
}
