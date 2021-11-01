using System.Collections.Generic;
using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.General
{
    [ScriptName("L_NS_LUP_TELEPORT.lua")]
    public class StarbaseTeleporter : BaseWorldTeleporter
    {
        protected override string TargetSpawnLocation => "NS_LW";
        protected override string Animation => "lup-teleport";

        public StarbaseTeleporter(GameObject gameObject) : base(gameObject)
        {
            var confirmationText = new Dictionary<int, string>
            {
                { 1200, "%[UI_TRAVEL_TO_NS]" },
                { 1900, "%[UI_TRAVEL_TO_NEXUS_TOWER]" },
            };

            Listen(gameObject.OnChoiceBoxRespond, (player, message) =>
            {
                var targetZone = int.Parse(message.ButtonIdentifier.Split("_")[1]);
                if (!confirmationText.TryGetValue(targetZone, out var text))
                    text = $"Do you want travel to zone {targetZone}?";

                // Confirmation popup to check if the player really wants to travel to [zone]
                this.ShowTransferPopup(player, targetZone, text);
            });
        }

        // Initial handler that shows the choice box for NS/NT
        protected override void ShowTransferPopup(Player player)
        {
            player.MessageGuiAsync("QueueChoiceBox", new Dictionary<string, object>
            {
                { "callbackClient", GameObject.Id.ToString() },
                {
                    "options", new object[]
                    {
                        new Dictionary<string, object>
                        {
                            { "caption", "%[UI_CHOICE_NS]" },
                            { "identifier", "zoneID_1200" },
                            { "image", "textures/ui/zone_thumnails/Nimbus_Station.dds" },
                            { "tooltipText", "%[UI_CHOICE_NS_HOVER]" },
                        },
                        new Dictionary<string, object>
                        {
                            { "caption", "%[UI_CHOICE_NT]" },
                            { "identifier", "zoneID_1900" },
                            { "image", "textures/ui/zone_thumnails/Nexus_Tower.dds" },
                            { "tooltipText", "%[UI_CHOICE_NT_HOVER]" },
                        },
                    }
                },
                { "strIdentifier", "choiceDoor" },
                { "title", "%[UI_CHOICE_DESTINATION]" },
            });
        }
    }
}
