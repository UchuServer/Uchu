using System.Collections.Generic;
using InfectedRose.Lvl;
using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Native implementation of scripts/zone/ag/l_zone_ag_survival.lua
    /// </summary>
    [ScriptName("l_zone_ag_survival.lua")]
    public class AvantGardensSurvival : BaseSurvivalGame
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public AvantGardensSurvival(GameObject gameObject) : base(gameObject)
        {
            var component = gameObject.GetComponent<LuaScriptComponent>();
            component.Data = new LegoDataDictionary {{"NumberOfPlayers", 4L, 8}};

            Listen(Zone.OnPlayerLoad,(player) =>
            {
                Listen(player.OnWorldLoad, () =>
                {
                    this.SetNetworkVar("Define_Player_To_UI", player.Id, 13);
                    this.SetNetworkVar("Show_ScoreBoard", 1, 7);
                    this.SetNetworkVar("Update_ScoreBoard_Players", new List<GameObject>() { player }, 13);
                });
                    
                var component = gameObject.GetComponent<ScriptedActivityComponent>();
                component.Participants.Add(player);
                Serialize(this.GameObject);
            });
        }
    }
}