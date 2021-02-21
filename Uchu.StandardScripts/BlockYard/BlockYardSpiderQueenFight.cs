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
using Uchu.Core.Resources;
using Uchu.World.Client;
using DestructibleComponent = Uchu.World.DestructibleComponent;
using Object = Uchu.World.Object;

namespace Uchu.StandardScripts.BlockYard
{
    [ZoneSpecific(1150)]
    public class BlockYardSpiderQueenFight : NativeScript
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
            "BirdFX",
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
                _fightCompleted = player.GetComponent<CharacterComponent>().GetFlag(FlagId.BeatSpiderQueen);
                
                if (!_globalSpawned)
                {
                    SpawnGlobal();
                    _globalSpawned = true;
                }
                
                if (_fightCompleted && !_peacefulSpawned)
                {
                    SpawnPeaceful(player);
                    _peacefulSpawned = true;
                }
                else if (!_fightCompleted && !_fightStarted)
                {
                    SpawnMaelstrom(player);

                    var spiderQueen = Zone.GameObjects.First(go => go.Lot == Lot.SpiderQueen);
                    var spiderQueenFight = SpiderQueenFight.Instantiate(Zone, spiderQueen, player);
                    spiderQueenFight.StartFight();
                    _fightStarted = true;
                    
                    Listen(spiderQueenFight.OnFightCompleted, () =>
                    {
                        player.GetComponent<CharacterComponent>().SetFlagAsync(FlagId.BeatSpiderQueen, true);
                        _fightCompleted = true;
                        _peacefulSpawned = true;
                        SpawnPeaceful(player);
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
            
            Logger.Debug("Spawned global");
        }
            
        private void SpawnMaelstrom(Player player)
        {
            StartFightEffects(player);
            
            // Destroy all maelstrom spawners
            foreach (var gameObject in Zone.GameObjects.Where(go => PeacefulObjects.Contains(go.Name)).ToArray())
            {
                // Destroy all spawned objects, except the spider queen which will be automatically destroyed afterwards
                if (gameObject.TryGetComponent<SpawnerComponent>(out var spawner))
                    foreach (var spawnedObject in spawner.ActiveSpawns.ToArray())
                        Destroy(spawnedObject);
                Destroy(gameObject);
            }
            
            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>()
                .Where(p => MaelstromObjects.Contains(p.PathName)))
            {
                Spawn(path);
            }
            
            Logger.Debug("Spawned maelstrom");
        }

        private void SpawnPeaceful(Player player)
        {
            StopFightEffects(player);
            
            // Destroy all maelstrom spawners
            foreach (var gameObject in Zone.GameObjects.Where(go => MaelstromObjects.Contains(go.Name)).ToArray())
            {
                // Destroy all spawned objects, except the spider queen which will be automatically destroyed afterwards
                if (gameObject.TryGetComponent<SpawnerComponent>(out var spawner) && gameObject.Name != "SpiderBoss")
                    foreach (var spawnedObject in spawner.ActiveSpawns.ToArray())
                        Destroy(spawnedObject);
                Destroy(gameObject);
            }
            
            // Create all peaceful spawners
            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>()
                .Where(p => PeacefulObjects.Contains(p.PathName)))
            {
                Spawn(path);
            }
            
            Logger.Debug("Spawned peaceful");
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
        
        private void StartFightEffects(Player player)
        {
            var maelStromFxObject = Zone.GameObjects.First(go => go.Lot == Lot.TornadoBgFx);
            
            player.Message(new PlayNDAudioEmitterMessage
            {
                Associate = player,
                NDAudioEventGUID = GuidMaelstrom.ToString()
            });

            
            player.Message(new PlayFXEffectMessage
            {
                Name = "TornadoDebris",
                EffectType = "debrisOn",
                Associate = maelStromFxObject
            });
                    
            player.Message(new PlayFXEffectMessage
            {
                Name = "TornadoVortex",
                EffectType = "VortexOn",
                Associate = maelStromFxObject
            });
                    
            player.Message(new PlayFXEffectMessage
            {
                Name = "silhouette",
                EffectType = "onSilhouette",
                Associate = maelStromFxObject
            });
        }

        private void StopFightEffects(Player player)
        {
            var maelStromFxObject = Zone.GameObjects.First(go => go.Lot == Lot.TornadoBgFx);
            
            player.Message(new PlayNDAudioEmitterMessage
            {
                Associate = player,
                NDAudioEventGUID = GuidPeaceful.ToString()
            });
            
            player.Message(new PlayFXEffectMessage
            {
                Name = "TornadoDebris",
                EffectType = "debrisOff",
                Associate = maelStromFxObject
            });
                    
            player.Message(new PlayFXEffectMessage
            {
                Name = "TornadoVortex",
                EffectType = "VortexOff",
                Associate = maelStromFxObject
            });
                    
            player.Message(new PlayFXEffectMessage
            {
                Name = "silhouette",
                EffectType = "offSilhouette",
                Associate = maelStromFxObject
            });
            
            Destroy(maelStromFxObject);
        }
        
        /// <summary>
        /// Represents one spider queen boss fight
        /// </summary>
        public class SpiderQueenFight : Object
        {
            public static SpiderQueenFight Instantiate(Zone zone, GameObject spiderQueen, Player player)
            {
                var instance = Instantiate<SpiderQueenFight>(zone);
                instance._player = player;
                instance._spiderQueen = spiderQueen;
                return instance;
            }
            
            private SpiderQueenFight()
            {
                OnFightCompleted = new Event();
            }
            
            /// <summary>
            /// Event called when the player completes the fight
            /// </summary>
            public Event OnFightCompleted { get; set; }

            /// <summary>
            /// The participant in this spider queen fight
            /// </summary>
            private Player _player;

            /// <summary>
            /// The spider queen currently active for the participants in the fight
            /// </summary>
            private GameObject _spiderQueen;

            #region state
            /// <summary>
            /// Starts the spider queen fight with the participants
            /// </summary>
            public void StartFight()
            {
                // Stop the fight if the spider queen was killed
                Listen(_spiderQueen.GetComponent<DestroyableComponent>().OnHealthChanged, async 
                    (newHealth, delta) =>
                {
                    if (newHealth <= 0)
                        await OnFightCompleted.InvokeAsync();
                });

                Listen(_player.OnFireServerEvent, (name, message) =>
                {
                    if (message.Arguments == "CleanupSpiders")
                        CleanupSpiders();
                });
            }

            #endregion state

            #region ai
            /// <summary>
            /// Removes all the spiders from the scene
            /// </summary>
            private void CleanupSpiders()
            {
                foreach (var gameObject in Zone.GameObjects.Where(i => i.Lot == 16197))
                    Destroy(gameObject);
            }
            
            private async Task WithdrawSpider(bool withdraw)
            {
                if (withdraw)
                {
                    // The animation handles movement
                    _player.Message(new SetStunnedMessage
                    {
                        Associate = _spiderQueen,
                        CantMove = true,
                        CantJump = true,
                        CantAttack = true,
                        CantTurn = true,
                        CantUseItem = true,
                        CantEquip = true,
                        CantInteract = true
                    });

                    // Orientation for the animation to make sense
                    _spiderQueen.Transform.Rotate(new Quaternion { X = 0.0f, Y = -0.005077f, Z = 0.0f, W = 0.999f });
                    _spiderQueen.Animate("withdraw");

                    var animation = (await ClientCache.GetTableAsync<Animations>()).FirstOrDefault(
                        a => a.Animationname == "withdraw"
                    );

                    var lengthMs = ((animation.Animationlength ??= 2.5f) - 0.25f) * 1000;

                    _player.Message(new SetStatusImmunityMessage
                    {
                        Associate = _spiderQueen,
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