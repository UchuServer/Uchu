using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    /// Native implementation of scripts/02_client/map/ve/l_mission_console_client.lua
    /// </summary>
    [ScriptName("l_mission_console_client.lua")]
    public class MissionConsole : ObjectScript
    {
        /// <summary>
        /// List of all the consoles in the zone.
        /// </summary>
        private static List<GameObject> _consoles = new List<GameObject>();
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public MissionConsole(GameObject gameObject) : base(gameObject)
        {
            _consoles.Add(this.GameObject);
            
            // Determine the flag name.
            if (!gameObject.Settings.TryGetValue("num", out var number)) return;
            var consoleFlag = int.Parse($"101{number}");
            
            // Listen to the player interacting.
            Listen(gameObject.OnInteract, async player =>
            {
                if (!player.TryGetComponent<CharacterComponent>(out var characterComponent)) return;
                if (!player.TryGetComponent<InventoryManagerComponent>(out var inventoryComponent)) return;
                if (characterComponent.GetFlag(consoleFlag)) return;
                
                // Add the mission item to the player.
                await characterComponent.SetFlagAsync(consoleFlag, true);
                await inventoryComponent.AddLotAsync(12547, 1);
                
                // Update the status of the console on the client.
                this.Zone.BroadcastMessage(new NotifyClientObjectMessage()
                {
                    Associate = this.GameObject,
                    Name = "",
                });
            });
        }

        /// <summary>
        /// Callback that is run once with the first GameObject created.
        /// </summary>
        public override void CompleteOnce()
        {
            // Listen for the console mission being started (clear the flags for the consoles).
            Listen(Zone.OnPlayerLoad, player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent)) return;
                if (!player.TryGetComponent<CharacterComponent>(out var characterComponent)) return;
                
                Listen(missionInventoryComponent.OnAcceptMission, (mission) =>
                {
                    if (mission.MissionId != (int) MissionId.DoYouSpeakChaos && mission.MissionId != (int) MissionId.DoYouSpeakChaos2) return;
                    Task.Run(async () =>
                    {
                        // Reset the console flags.
                        for (var i = 0; i < 10; i++)
                        {
                            await characterComponent.SetFlagAsync(int.Parse($"101{i}"), false);
                        }
                        
                        // Notify the changes to the consoles.
                        foreach (var console in _consoles)
                        {
                            this.Zone.BroadcastMessage(new NotifyClientObjectMessage()
                            {
                                Associate = console,
                                Name = "",
                            });
                        }
                    });
                });
            });
        }
    }
}