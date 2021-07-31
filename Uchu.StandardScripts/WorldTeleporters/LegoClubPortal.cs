using System.Collections.Generic;
using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.WorldTeleporters
{
    [ScriptName("ScriptComponent_1239_script_name__removed")]
    [ScriptName("ScriptComponent_1485_script_name__removed")]
    public class LegoClubPortal : BaseWorldTeleporter
    {
        protected override string MessageBoxText => this.TargetZone switch
        {
            1200 => "%[UI_TRAVEL_TO_NS]",
            1900 => "%[UI_TRAVEL_TO_NEXUS_TOWER]",
            _ => null,
        };
        protected override int TargetZone => int.Parse((string) this.GameObject.Settings["transferZoneID"]);
        protected override string Animation => "lup-teleport";
        protected override string AcceptIdentifier => "PlayButton";
        protected override string CancelIdentifier => "CloseButton";
        protected override string TargetSpawnLocation => this.TargetZone switch
        {
            1200 => "NS_LEGO_Club",
            1900 => "NS_LEGO_Club",
            _ => null,
        };
        protected override bool CheckUserData => false;

        protected override void ShowTransferPopup(Player player)
        {
            // Show special popup for LEGO® Club
            if (this.TargetZone == 1700)
                player.MessageGuiAsync("pushGameState", new Dictionary<string, object>
                {
                    {"context", new Dictionary<string, object>
                    {
                        {"HelpVisible", "show"},
                        {"callbackObj", this.GameObject.Id.ToString()},
                        {"type", "Lego_Club_Valid"},
                        {"user", player.Id.ToString()},
                    }},
                    {"state", "Lobby"},
                });
            // Show default confirmation message box
            else
                base.ShowTransferPopup(player);
        }

        public LegoClubPortal(GameObject gameObject) : base(gameObject)
        {
        }
    }
}
