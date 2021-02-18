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
using System.Collections.Generic;
using InfectedRose.Luz;
using System.Numerics;
using Uchu.World.Client;
using DestructibleComponent = Uchu.World.DestructibleComponent;

namespace Uchu.StandardScripts.BlockYard
{
    [ZoneSpecific(1150)]
    public class BlockYardProperty : NativeScript
    {
        /// <summary>
        /// Ambient sound when maelstrom battle is active
        /// </summary>
        private static readonly Guid GuidMaelstrom = new Guid("{7881e0a1-ef6d-420c-8040-f59994aa3357}");
        
        /// <summary>
        /// Ambient sound when maelstrom battle is over
        /// </summary>
        private static readonly Guid GuidPeaceful = new Guid("{c5725665-58d0-465f-9e11-aeb1d21842ba}");

        /// <summary>
        /// Objects that are always present
        /// </summary>
        private static readonly string[] GlobalObjects = {
            "Mailbox",
            "PropertyGuard",
            "Launcher"
        };
        
        /// <summary>
        /// Objects needed for maelstrom battle
        /// </summary>
        private static readonly string[] MaelstromObjects = {
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
        
        /// <summary>
        /// Objects needed once Maelstrom battle is over
        /// </summary>
        private static readonly string[] PeacefulObjects = {
            "SunBeam",
            "BankObj",
            "AGSmallProperty"
        };

        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                var spiderQueenFight = new SpiderQueenFight(this, player);
                spiderQueenFight.StartFight();
            });

            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Represents one spider queen boss fight
        /// </summary>
        private class SpiderQueenFight
        {
            public SpiderQueenFight(BlockYardProperty blockYard, Player player)
            {
                _blockYard = blockYard;
                _player = player;
                _maelstromObjects = new List<GameObject>();
                _fightCompleted = false;
            }

            /// <summary>
            /// Whether the player has completed the fight
            /// </summary>
            private bool _fightCompleted;

            /// <summary>
            /// The parent script
            /// </summary>
            private readonly BlockYardProperty _blockYard;

            /// <summary>
            /// The participant in this spider queen fight
            /// </summary>
            private readonly Player _player;

            /// <summary>
            /// The spider eggs currently active for the participants in the fight
            /// </summary>
            private readonly List<GameObject> _maelstromObjects;
            
            /// <summary>
            /// The spider queen currently active for the participants in the fight
            /// </summary>
            private GameObject SpiderQueen { get; set; }
            
            #region state
            /// <summary>
            /// Starts the spider queen fight with the participants
            /// </summary>
            public void StartFight()
            {
                _player.Message(new PlayNDAudioEmitterMessage
                {
                    Associate = _player,
                    NDAudioEventGUID = GuidMaelstrom.ToString()
                });

                // Spawn all the spawners
                _maelstromObjects.AddRange(SpawnPaths(true));

                foreach (var gameObject in _blockYard.Zone.GameObjects)
                {
                    switch (gameObject.Lot)
                    {
                        case Lot.SpiderQueenEgg:
                            _maelstromObjects.Add(gameObject);
                            break;
                        case Lot.BuildBorder:
                            Destroy(gameObject);
                            break;
                        case Lot.SpiderQueen:
                            gameObject.RemoveComponent<BaseCombatAiComponent>();
                            SpiderQueen = gameObject;
                            _maelstromObjects.Add(SpiderQueen);
                            break;
                        case Lot.Spawner:
                            if (gameObject.TryGetComponent<SpawnerComponent>(out var spawner) && spawner.SpawnTemplate == Lot.SpiderQueen)
                                _maelstromObjects.Add(gameObject);
                            break;
                        case Lot.TornadoBgFx:
                            _player.Message(new PlayFXEffectMessage
                            {
                                Name = "TornadoDebris",
                                EffectType = "debrisOn",
                                Associate = gameObject
                            });
                    
                            _player.Message(new PlayFXEffectMessage
                            {
                                Name = "TornadoVortex",
                                EffectType = "VortexOn",
                                Associate = gameObject
                            });
                    
                            _player.Message(new PlayFXEffectMessage
                            {
                                Name = "silhouette",
                                EffectType = "onSilhouette",
                                Associate = gameObject
                            });
                            break;
                    }
                }

                // Stop the fight if the spider queen was killed
                _blockYard.Listen(SpiderQueen.GetComponent<DestructibleComponent>().OnSmashed, 
                    (spiderQueen, player) =>
                {
                    EndFight();
                });

                _blockYard.Listen(_player.OnFireServerEvent, (name, message) =>
                {
                    if (message.Arguments == "CleanupSpiders")
                        CleanupSpiders();
                });
            }

            /// <summary>
            /// Ends the fight
            /// </summary>
            private void EndFight()
            {
                var zone = _blockYard.Zone;

                foreach (var gameObject in _maelstromObjects)
                    Destroy(gameObject);

                foreach (var gameObject in zone.GameObjects.Where(i => i.Lot == Lot.TornadoBgFx))
                    KillEffects(gameObject);
                
                // Play peaceful sounds
                _player.Message(new PlayNDAudioEmitterMessage
                {
                    Associate = _player,
                    NDAudioEventGUID = GuidPeaceful.ToString()
                });

                // Spawns all the peaceful objects
                SpawnPaths(false);

                foreach (var item in zone.GameObjects.Where(i => i.Lot == Lot.TornadoBgFx))
                {
                    _player.Message(new StopFXEffectMessage
                    {
                        Name = "beam",
                        Associate = item,
                        KillImmediate = false
                    });

                    _player.Message(new DieMessage
                    {
                        Associate = item,
                        ClientDeath = true,
                        SpawnLoot = true,
                        DeathType = "",
                        DirectionRelativeAngleXz = 0.00f,
                        DirectionRelativeAngleY = 0.00f,
                        DirectionRelativeForce = 0.00f,
                        KillType = 1,
                        Killer = default,
                        LootOwner = default
                    });
                }
            }

            #endregion state

            #region utilities
                        /// <summary>
            /// Removes all the special effects from the scene
            /// </summary>
            /// <param name="gameObject">The effect game object</param>
            private void KillEffects(GameObject gameObject)
            {
                _player.Message(new StopFXEffectMessage
                {
                    Name = "TornadoVortex",
                    Associate = gameObject,
                    KillImmediate = true
                });

                _player.Message(new StopFXEffectMessage
                {
                    Name = "TornadoDebris",
                    Associate = gameObject,
                    KillImmediate = true
                });

                _player.Message(new StopFXEffectMessage
                {
                    Name = "silhouette",
                    Associate = gameObject,
                    KillImmediate = true
                });

                _player.Message(new DieMessage
                {
                    Associate = gameObject,
                    ClientDeath = true,
                    SpawnLoot = true,
                    DeathType = "",
                    DirectionRelativeAngleXz = 0.00f,
                    DirectionRelativeAngleY = 0.00f,
                    DirectionRelativeForce = 0.00f,
                    KillType = 1,
                    Killer = default,
                    LootOwner = default
                });
            }
            
            private List<GameObject> SpawnPaths(bool isMaelstrom)
            {
                var spawnedObjects = new List<GameObject>();
                
                foreach (var path in _blockYard.Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>())
                {
                    try
                    {
                        if (isMaelstrom ? MaelstromObjects.Contains(path.PathName) : PeacefulObjects.Contains(path.PathName)
                            || GlobalObjects.Contains(path.PathName))
                        {

                            var obj = InstancingUtilities.Spawner(path, _blockYard.Zone);
                            if (obj == null)
                                continue;
                            
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
                            spawnedObjects.Add(obj);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Warning(e);
                    }
                }

                return spawnedObjects;
            }
            #endregion utilities

            #region ai
            /// <summary>
            /// Removes all the spiders from the scene
            /// </summary>
            private void CleanupSpiders()
            {
                foreach (var gameObject in _blockYard.Zone.GameObjects.Where(i => i.Lot == 16197))
                    Destroy(gameObject);
            }
            
            private async Task WithdrawSpider(bool withdraw)
            {
                if (withdraw)
                {
                    // The animation handles movement
                    _player.Message(new SetStunnedMessage
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

                    // Orientation for the animation to make sense
                    SpiderQueen.Transform.Rotate(new Quaternion { X = 0.0f, Y = -0.005077f, Z = 0.0f, W = 0.999f });
                    SpiderQueen.Animate("withdraw");

                    var animation = (await ClientCache.GetTableAsync<Animations>()).FirstOrDefault(
                        a => a.Animationname == "withdraw"
                    );

                    var lengthMs = ((animation.Animationlength ??= 2.5f) - 0.25f) * 1000;

                    _player.Message(new SetStatusImmunityMessage
                    {
                        Associate = SpiderQueen,
                        state = ImmunityState.Push,
                        bImmuneToSpeed = true,
                        bImmuneToBasicAttack = true,
                        bImmuneToDOT = true
                    });
                }
            }
            #endregion ai
        }
    }
}