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
        static bool HasOwner { get; set; } = true;
        static private string[] GlobalObjects { get; } =
        {
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
            "ROF_Targets_04",
            "RFS_Targets"
        };
        static private string[] PeacefulObjects { get; } =
        {
            "ShowProperty",
            "SunBeam",
            "BankObj"
        };
        private GameObject[] SpiderEggs { get; set; } = { };
        private GameObject SpiderQueen { get; set; }
        private Player player { get; set; }
        

        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, (LocalPlayer) =>
            {
                player = LocalPlayer;
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
            Task task = player.SetFlagAsync(71, true);
            player.Message(new PlayNDAudioEmitterMessage
            {
                Associate = player,
                NDAudioEventGUID = GUIDPeaceful
            }); // Play peaceful sounds

            SpawnPaths(false);
        }

        private void Maelstrom(Player player)
        {
            player.Message(new PlayNDAudioEmitterMessage
            {
                Associate = player,
                NDAudioEventGUID = GUIDMaelstrom
            }); // Play eerie sounds

            SpawnPaths(true);

            foreach (var item in Zone.GameObjects)
            {
                if (item.Lot.Id == 14375)
                {
                    SpiderEggs.Append(item); // Spider Queen Eggs
                }

                if (item.Lot.Id == 9524) Destroy(item); // Build Border (We don't need it atm)

                if (item.Lot.Id == 14381) // Spider Queen
                {
                    item.RemoveComponent<World.BaseCombatAiComponent>(); // Once the Combat AI is fixed we can remove this
                    SpiderQueen = item;
                }
                
                if (item.Lot.Id == 9938)
                {
                    player.Message(new PlayFXEffectMessage
                    {
                        Name = "TornadoDebris",
                        EffectType = "debrisOn",
                        Associate = item
                    });
                    player.Message(new PlayFXEffectMessage
                    {
                        Name = "TornadoVortex",
                        EffectType = "VortexOn",
                        Associate = item
                    });
                    player.Message(new PlayFXEffectMessage
                    {
                        Name = "silhouette",
                        EffectType = "onSilhouette",
                        Associate = item
                    });
                }
            }

            Listen(player.OnFireServerEvent, (name, message) =>
            {
                if (message.Arguments == "CleanupSpiders") CleanupSpiders();
                
            });
        }

        private void CleanupSpiders()
        {
            foreach (var item in Zone.GameObjects) if (item.Lot.Id == 16197) Destroy(item);
        }

        private void EggsToSpiders()
        {

        }

        private void WithdrawSpider(Player player, bool Withdraw)
        {
            if (Withdraw)
            {
                player.Message(new SetStunnedMessage
                {
                    Associate = SpiderQueen,
                    CantMove = true,
                    CantJump = true,
                    CantAttack = true,
                    CantTurn = true,
                    CantUseItem = true,
                    CantEquip = true,
                    CantInteract = true
                });

                SpiderQueen.Transform.Rotate(new Quaternion { X = 0.0f, Y = -0.005077f, Z = 0.0f, W = 0.999f });

                SpiderQueen.Animate("withdraw");
                
                using var cdClient = new CdClientContext();
                var Animation = cdClient.AnimationsTable.FirstOrDefault(
                    a => a.Animationname == "withdraw"
                );

                float LengthMs = ((Animation.Animationlength ??= 2.5f) - 0.25f) * 1000;

                player.Message(new SetStatusImmunityMessage
                {
                    Associate = SpiderQueen,
                    state = ImmunityState.Push,
                    bImmuneToSpeed = true,
                    bImmuneToBasicAttack = true,
                    bImmuneToDOT = true
                });
            }
        }

        private void TriggerTimerEvents(string TimerName)
        {

        }

        private void SpawnPaths(bool IsMaelstrom)
        {
            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>())
            {
                try
                {
                    if (IsMaelstrom ? MaelstromObjects.Contains(path.PathName) : PeacefulObjects.Contains(path.PathName) || GlobalObjects.Contains(path.PathName))
                    {

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
        }

        private void DestroyMaelstrom()
        {
            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>())
            {
                try
                {
                    if (MaelstromObjects.Contains(path.PathName))
                    {

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
                if (item.Lot.Id == 9938)
                {
                    player.Message(new StopFXEffectMessage
                    {
                        Name = "TornadoVortex",
                        Associate = item,
                        KillImmediate = true
                    });
                    player.Message(new StopFXEffectMessage
                    {
                        Name = "TornadoDebris",
                        Associate = item,
                        KillImmediate = true
                    });
                    player.Message(new StopFXEffectMessage
                    {
                        Name = "silhouette",
                        Associate = item,
                        KillImmediate = true
                    });
                }
            }
        }
    }
}