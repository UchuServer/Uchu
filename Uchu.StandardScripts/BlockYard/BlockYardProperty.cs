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
using IronPython.Modules;
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
        private static readonly HashSet<string> GlobalObjects = new HashSet<string> {
            "Mailbox",
            "PropertyGuard",
            "Launcher"
        };
        
        /// <summary>
        /// Objects needed for maelstrom battle
        /// </summary>
        private static readonly HashSet<string> MaelstromObjects = new HashSet<string> {
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
        private static readonly HashSet<string> PeacefulObjects = new HashSet<string> {
            "SunBeam",
            "BankObj",
            "AGSmallProperty"
        };

        /// <summary>
        /// Whether the player has started the fight yet
        /// </summary>
        private bool _fightStarted;

        /// <summary>
        /// Whether the player has completed the fight yet
        /// </summary>
        private bool _fightCompleted;

        /// <summary>
        /// Whether the global objects have been spawned
        /// </summary>
        private bool _globalSpawned;

        private bool _peacefulSpawned;

        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                if (!_globalSpawned)
                {
                    SpawnGlobal();
                    _globalSpawned = true;
                }
                
                if (_fightCompleted && !_peacefulSpawned)
                {
                    SpawnPeaceful();
                    _peacefulSpawned = true;
                }
                else if (!_fightCompleted && !_fightStarted)
                {
                    SpawnMaelstrom();

                    var spiderQueenFight = new SpiderQueenFight(this, player);
                    spiderQueenFight.StartFight();
                    _fightStarted = true;
                    
                    Listen(spiderQueenFight.OnFightCompleted, () =>
                    {
                        _fightCompleted = true;
                        _peacefulSpawned = true;
                        SpawnPeaceful();
                    });
                }
            });

            return Task.CompletedTask;
        }
        
        private void SpawnGlobal()
        {
            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>()
                .Where(p => GlobalObjects.Contains(p.PathName)))
            {
                Spawn(path);
            }
        }
            
        private void SpawnMaelstrom()
        {
            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>()
                .Where(p => MaelstromObjects.Contains(p.PathName)))
            {
                Spawn(path);
            }
        }

        private void SpawnPeaceful()
        {
            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>()
                .Where(p => PeacefulObjects.Contains(p.PathName)))
            {
                Spawn(path);
            }
        }

        private void Spawn(LuzSpawnerPath path)
        {
            var gameObject = InstancingUtilities.Spawner(path, Zone);
            if (gameObject == null)
                return;
            
            gameObject.Layer = StandardLayer.Hidden;

            var spawner = gameObject.GetComponent<SpawnerComponent>();
            spawner.SpawnsToMaintain = (int)path.NumberToMaintain;
            spawner.SpawnLocations = path.Waypoints.Select(w => new SpawnLocation
            {
                Position = w.Position,
                Rotation = Quaternion.Identity
            }).ToList();

            Start(gameObject);
            spawner.SpawnCluster();
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
                OnFightCompleted = new Event();
            }
            
            /// <summary>
            /// Event called when the player completes the fight
            /// </summary>
            public Event OnFightCompleted { get; set; }

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

                foreach (var gameObject in _blockYard.Zone.GameObjects)
                {
                    // TODO: Hide spider queen ROF target
                    switch (gameObject.Lot)
                    {
                        case Lot.SpiderQueenEgg:
                            _maelstromObjects.Add(gameObject);
                            break;
                        case Lot.Spawner:
                            if (MaelstromObjects.Contains(gameObject.Name))
                                _maelstromObjects.Add(gameObject);
                            break;
                        case Lot.BuildBorder:
                            Destroy(gameObject);
                            break;
                        case Lot.SpiderQueen:
                            SpiderQueen = gameObject;
                            break;
                        case Lot.TornadoBgFx:
                            _maelstromObjects.Add(gameObject);
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
                _blockYard.Listen(SpiderQueen.GetComponent<DestroyableComponent>().OnHealthChanged, async 
                    (newHealth, delta) =>
                {
                    if (newHealth <= 0)
                        await EndFightAsync();
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
            private async Task EndFightAsync()
            {
                var zone = _blockYard.Zone;

                foreach (var gameObject in _maelstromObjects)
                    Destroy(gameObject);

                // Play peaceful sounds
                _player.Message(new PlayNDAudioEmitterMessage
                {
                    Associate = _player,
                    NDAudioEventGUID = GuidPeaceful.ToString()
                });

                await OnFightCompleted.InvokeAsync();
            }

            #endregion state

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