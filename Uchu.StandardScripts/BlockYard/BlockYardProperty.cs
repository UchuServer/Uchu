using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.StandardScripts;
using System.IO;
using System;
using InfectedRose.Luz;
using System.Numerics;

namespace Uchu.StandardScripts.BlockYard
{
    [ZoneSpecific(1150)]
    public class BlockYardProperty : NativeScript
    {
        static private string GUIDMaelstrom { get; } = "{7881e0a1-ef6d-420c-8040-f59994aa3357}"; // ambient sounds for when the Maelstrom is on
        static private string GUIDPeaceful { get; } = "{c5725665-58d0-465f-9e11-aeb1d21842ba}"; // happy ambient sounds when no Maelstrom is present
        static bool HasOwner { get; set; } = false;
        static private string[] GlobalObjects { get; } =
        {
            "FXObject",
            "Mailbox",
            "PropertyGuard",
            "Launcher"
        };
        static private string[] MaelstromObjects { get; } =
        {
            "DestroyMaelstrom",
            "SpiderBoss",
            "SpiderEggs",
            "Rocks",
            "DesMaelstromInstance",
            "Spider_Scream",
            "ROF_Targets_00",
            "ROF_Targets_01",
            "ROF_Targets_02",
            "ROF_Targets_03",
            "ROF_Targets_04"
        };
        static private string[] PeacefulObjects { get; } =
        {

        };

        

        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, (player) =>
            {
                if (HasOwner) // If the world is peaceful
                {
                    Peaceful(player);
                } 
                else
                {
                    Maelstrom(player);
                }
            });

            return Task.CompletedTask;
        }

        private void Peaceful(Player player)
        {
            player.Message(new PlayNDAudioEmitterMessage
            {
                Associate = player,
                NDAudioEventGUID = GUIDPeaceful
            }); // Play peaceful sounds
        }

        private void Maelstrom(Player player)
        {
            player.Message(new PlayNDAudioEmitterMessage
            {
                Associate = player,
                NDAudioEventGUID = GUIDMaelstrom
            }); // Play eerie sounds

            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>())
            {
                try
                {
                    if (MaelstromObjects.Contains(path.PathName) || GlobalObjects.Contains(path.PathName)) {
                        var obj = InstancingUtilities.Spawner(path, Zone);

                        if (obj == null) return;

                        obj.Layer = StandardLayer.Hidden;

                        var spawner = obj.GetComponent<SpawnerComponent>();

                        spawner.SpawnsToMaintain = (int)path.NumberToMaintain;

                        spawner.SpawnLocations = path.Waypoints.Select(w => new SpawnLocation
                        {
                            Position = w.Position,
                            Rotation = Quaternion.Identity
                        }).ToList();

                        Start(obj);

                        spawner.SpawnCluster();
                    }
                }
                catch (Exception e)
                {
                    Logger.Warning(e);
                }
            }

            foreach (var item in Zone.GameObjects)
            {
                if (item.Lot.Id == 14375)
                {
                    var destructibleComponent = (World.DestructibleComponent)item.GetComponent(typeof(World.DestructibleComponent));

                    Listen(destructibleComponent.OnSmashed, (killer, lootOwner) =>
                    {
                        var spider = GameObject.Instantiate(Zone, 16197, item.Transform.Position);

                        Start(spider);

                        Construct(spider);
                    });
                }
                if (item.Lot.Id == 9524)
                {
                    Destroy(item);
                }  
            }
        }
    }
}