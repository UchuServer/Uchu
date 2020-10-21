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
        static private string GUIDPeacful { get; } = "{c5725665-58d0-465f-9e11-aeb1d21842ba}"; // happy ambient sounds when no Maelstrom is present
        static bool HasOwner { get; set; } = false;
        static private string[] MaelstromObjects { get; set; } =
        {
            "StrombieWander",
            "Strombies",
            "ClaimMarker",
            "Generator",
            "MaelstromFX",
            "MaelstromSpots",
            "PropertyGuard",
            "ClaimMarker",
            "Generator",
            "Guard",
            "PropertyPlaque",
            "PropertyVendor",
            "Spots",
            "maelstrom",
            "strombies",
            "FXObject",
            "Mailbox"

        };
        

        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, (player) =>
            {
                if (HasOwner)
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
                NDAudioEventGUID = GUIDPeacful
            });
        }

        private void Maelstrom(Player player)
        {
            player.Message(new PlayNDAudioEmitterMessage
            {
                Associate = player,
                NDAudioEventGUID = GUIDMaelstrom
            });

            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>())
            {
                try
                {
                    if (MaelstromObjects.Contains(path.PathName)) {
                        SpawnPath(path);
                    }
                }
                catch (Exception e)
                {
                    Logger.Warning(e);
                }
            }
        }

        private void SpawnPath(LuzSpawnerPath spawnerPath)
        {
            var obj = InstancingUtilities.Spawner(spawnerPath, Zone);

            if (obj == null) return;

            obj.Layer = StandardLayer.Hidden;

            var spawner = obj.GetComponent<SpawnerComponent>();

            spawner.SpawnsToMaintain = (int)spawnerPath.NumberToMaintain;

            spawner.SpawnLocations = spawnerPath.Waypoints.Select(w => new SpawnLocation
            {
                Position = w.Position,
                Rotation = Quaternion.Identity
            }).ToList();

            Start(obj);

            spawner.SpawnCluster();
        }
    }
}