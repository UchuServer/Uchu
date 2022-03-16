using System.Collections.Generic;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1528_script_name__removed")]
    public class DirtCloud : ObjectScript
    {
        private static readonly Dictionary<string, int> Missions = new()
        {
            {"Dirt_Clouds_Sent",1734},
            {"Dirt_Clouds_Assem",1776},
            {"Dirt_Clouds_Para",1777},
            {"Dirt_Clouds_Halls",1783},
        };
        public DirtCloud(GameObject gameObject) : base(gameObject)
        {
            foreach (var p in Zone.Players) Event(p);
            //since these dirt clouds are spawned and removed frequently, does this cause a memory leak?
            Listen(Zone.OnPlayerLoad, Event);
        }
        private void Event(Player player)
        {
            Listen(player.OnSkillEvent, async (target, effectHandler) =>
            {
                if (target == GameObject && effectHandler == "soapspray")
                {
                    if (GameObject.Spawner != null)
                    {
                        var spawner = GameObject.Spawner.GameObject.Name;
                        var missionInventoryComponent = player.GetComponent<MissionInventoryComponent>();
                        await missionInventoryComponent.ScriptAsync(1862, GameObject.Lot);
                        if (Missions.ContainsKey(spawner))
                            await missionInventoryComponent.ScriptAsync(Missions[spawner], GameObject.Lot);
                    }
                    Zone.BroadcastMessage(new DieMessage
                    {
                        Associate = GameObject,
                        Killer = player,
                        KillType = KillType.Violent,
                        SpawnLoot = false,
                    });
                    Destroy(GameObject);
                }
            });
        }
    }
}