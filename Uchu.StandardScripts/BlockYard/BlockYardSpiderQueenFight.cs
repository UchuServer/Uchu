using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;
using System;
using System.Collections.Generic;
using InfectedRose.Luz;
using System.Numerics;
using InfectedRose.Lvl;
using Uchu.Core.Resources;
using Uchu.Physics;
using Uchu.World.Client;
using DestructibleComponent = Uchu.World.DestructibleComponent;
using Object = Uchu.World.Object;
using PhysicsComponent = Uchu.World.PhysicsComponent;

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
        
        private static readonly HashSet<string> Volumes = new HashSet<string>
        {
            "Zone1Vol", 
            // "Zone2Vol", 
            // "Zone3Vol", 
            // "Zone4Vol", 
            // "Zone5Vol", 
            // "Zone6Vol", 
            // "Zone7Vol",
            // "Zone8Vol",
            "AggroVol",
            "TeleVol"
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
            "Land_Target",
            "ROF_Targets_00",
            "ROF_Targets_01",
            "ROF_Targets_02",
            "ROF_Targets_03",
            "ROF_Targets_04"
        }.Union(Volumes.ToArray()).ToHashSet();

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
            Listen(Zone.OnPlayerLoad, async player =>
            {
                _fightCompleted = player.GetComponent<CharacterComponent>().GetFlag(FlagId.BeatSpiderQueen);
                
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

                    var spiderQueen = Zone.GameObjects.First(go => go.Lot == Lot.SpiderQueen);
                    var spiderEggSpawner = Zone.GameObjects.First(go => go.Name == "SpiderEggs");
                    var aggroVolume = Zone.GameObjects.First(go => go.Name == "AggroVol")
                        .GetComponent<SpawnerComponent>().ActiveSpawns.First().GetComponent<PhysicsComponent>();
                    
                    var spiderQueenFight = await SpiderQueenFight.Instantiate(Zone, spiderQueen, spiderEggSpawner, aggroVolume);
                    spiderQueenFight.StartFight();
                    _fightStarted = true;
                    
                    Listen(spiderQueenFight.OnFightCompleted, () =>
                    {
                        foreach (var zonePlayer in Zone.Players)
                        {
                            zonePlayer.GetComponent<CharacterComponent>().SetFlagAsync(FlagId.BeatSpiderQueen, true);
                        }
                        
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
            StartFightEffects();

            // Destroy all maelstrom spawners
            foreach (var gameObject in Zone.GameObjects.Where(go => PeacefulObjects.Contains(go.Name)).ToArray())
            {
                // Destroy all spawned objects, except the spider queen which will be automatically destroyed afterwards
                if (gameObject.TryGetComponent<SpawnerComponent>(out var spawner))
                    foreach (var spawnedObject in spawner.ActiveSpawns.ToArray())
                        Destroy(spawnedObject);
                Destroy(gameObject);
            }
            
            // Spawn all the maelstrom objects
            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>()
                .Where(p => MaelstromObjects.Contains(p.PathName)))
            {
                Spawn(path);
            }

            // Hide the physics colliders
            foreach (var volumeSpawner in Zone.GameObjects.Where(go => Volumes.Contains(go.Name)))
            {
                foreach (var volume in volumeSpawner.GetComponent<SpawnerComponent>().ActiveSpawns)
                {
                    var physics = volume.AddComponent<PhysicsComponent>();
                    var cylinder = CylinderBody.Create(
                        Zone.Simulation,
                        volumeSpawner.Transform.Position,
                        volumeSpawner.Transform.Rotation,
                        new Vector2(45, 100)
                    );
                    physics.SetPhysics(cylinder);

                    var position = volume.Transform.Position;
                    position.Y -= 10;
                    volume.Transform.Position = position;
                    GameObject.Serialize(volume);
                }
            }
        }

        private void SpawnPeaceful()
        {
            StopFightEffects();
            
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
        
        private void StartFightEffects()
        {
            var maelStromFxObject = Zone.GameObjects.First(go => go.Lot == Lot.TornadoBgFx);
            
            Zone.BroadcastMessage(new PlayNDAudioEmitterMessage
            {
                NDAudioEventGUID = GuidMaelstrom.ToString()
            });

            
            Zone.BroadcastMessage(new PlayFXEffectMessage
            {
                Name = "TornadoDebris",
                EffectType = "debrisOn",
                Associate = maelStromFxObject
            });
                    
            Zone.BroadcastMessage(new PlayFXEffectMessage
            {
                Name = "TornadoVortex",
                EffectType = "VortexOn",
                Associate = maelStromFxObject
            });
                    
            Zone.BroadcastMessage(new PlayFXEffectMessage
            {
                Name = "silhouette",
                EffectType = "onSilhouette",
                Associate = maelStromFxObject
            });
        }

        private void StopFightEffects()
        {
            var maelStromFxObject = Zone.GameObjects.First(go => go.Lot == Lot.TornadoBgFx);
            
            Zone.BroadcastMessage(new PlayNDAudioEmitterMessage
            {
                NDAudioEventGUID = GuidPeaceful.ToString()
            });
            
            Zone.BroadcastMessage(new PlayFXEffectMessage
            {
                Name = "TornadoDebris",
                EffectType = "debrisOff",
                Associate = maelStromFxObject
            });
                    
            Zone.BroadcastMessage(new PlayFXEffectMessage
            {
                Name = "TornadoVortex",
                EffectType = "VortexOff",
                Associate = maelStromFxObject
            });
                    
            Zone.BroadcastMessage(new PlayFXEffectMessage
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
            public static async Task<SpiderQueenFight> Instantiate(Zone zone, GameObject spiderQueen, GameObject spiderEggSpawner,
                PhysicsComponent aggroVolume)
            {
                var instance = Instantiate<SpiderQueenFight>(zone);
                instance._spiderQueen = spiderQueen;
                instance._spiderEggSpawner = spiderEggSpawner; 
                instance._spiderQueenFactions = spiderQueen.GetComponent<DestroyableComponent>().Factions.ToArray();
                instance.Enabled = false;
                
                instance._stage2Threshold = spiderQueen.GetComponent<DestroyableComponent>().MaxHealth / 3 * 2;
                instance._stage3Threshold = spiderQueen.GetComponent<DestroyableComponent>().MaxHealth / 3;
                instance._safePlayers = new HashSet<Player>();

                instance._sensingRadius = 36;
                instance._stage2SpiderlingCount = 2;
                instance._stage3SpiderlingCount = 3;
                instance._spawnedSpiderlings = new List<GameObject>();
                instance._preppedSpiderEggs = new List<GameObject>();

                // Cache all the animation times for animations executed by the spider queen
                instance._tauntAnimationTime = await GetAnimationTimeAsync("taunt");
                instance._rainOfFireAnimationTime = await GetAnimationTimeAsync("attack-fire");
                instance._withdrawalTime = await GetAnimationTimeAsync("withdraw");
                instance._advanceAnimationTime = await GetAnimationTimeAsync("advance");
                instance._withdrawnIdleAnimationTime = await GetAnimationTimeAsync("idle-withdrawn");
                instance._shootLeftAnimationTime = await GetAnimationTimeAsync("attack-shoot-left");
                instance._shootRightAnimationTime = await GetAnimationTimeAsync("attack-shoot-right");
                instance._shootAnimationTime = await GetAnimationTimeAsync("attack-fire-single");
                instance.HandleAggroPhysics(aggroVolume);

                return instance;
            }

            private void HandleAggroPhysics(PhysicsComponent aggroVolume)
            {
                Listen(aggroVolume.OnEnter, other =>
                {
                    if (other.GameObject is Player player)
                        _safePlayers.Add(player);
                    if (Enabled && _safePlayers.Count == Zone.Players.Length)
                        Enabled = false;
                });

                Listen(aggroVolume.OnLeave, other =>
                {
                    if (other.GameObject is Player player)
                        _safePlayers.Remove(player);
                    if (!Enabled && _safePlayers.Count != Zone.Players.Length)
                        Enabled = true;
                });
            }

            private void SetupVolumePhysics()
            {
                var center = _spiderQueen.Transform.Position;
                foreach (var volume in Zone.GameObjects)
                {
                    var vector1 = Vector3.Zero;
                    var vector2 = Vector3.Zero;

                    switch (volume.Name)
                    {
                        case "Zone1Vol":
                            vector1 = new Vector3((float) (center.X + 0.5 * _sensingRadius), center.Y, center.Z);
                            vector2 = new Vector3(center.X + _sensingRadius, center.Y, (float) (center.Z + 0.5 * _sensingRadius));
                            break;
                    }

                    if (vector1 != Vector3.Zero && vector2 != Vector3.Zero)
                    {
                        // TODO
                    }
                }
            }

            private static async Task<float> GetAnimationTimeAsync(string animationName)
            {
                var animationTable = await ClientCache.GetTableAsync<Animations>();
                var advanceAnimationLength = animationTable.First(
                    a => a.Animationtype == animationName && a.AnimationGroupID == 541
                    ).Animationlength;
                return (advanceAnimationLength ?? 0) * 1000;
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
            /// The spider queen currently active for the participants in the fight
            /// </summary>
            private GameObject _spiderQueen;

            private int[] _spiderQueenFactions;

            private GameObject _spiderEggSpawner;

            private List<GameObject> _spawnedSpiderlings;
            private List<GameObject> _preppedSpiderEggs;
            private int _spiderEggsToPrep;

            private uint _stage = 1;
            private bool _withdrawn;
            private uint _stage2Threshold;
            private uint _stage3Threshold;
            private int _stage2SpiderlingCount;
            private int _stage3SpiderlingCount;
            private int _killedSpiders;
            private float _withdrawalTime;
            private float _advanceAnimationTime;
            private GameObject _advanceAttackTarget;
            private float _tauntAnimationTime;
            private float _rainOfFireAnimationTime;
            private float _withdrawnIdleAnimationTime;
            private float _shootLeftAnimationTime;
            private float _shootRightAnimationTime;
            private float _shootAnimationTime;
            private int _currentSpiderlingWavecount;
            private bool _enabled;
            private HashSet<Player> _safePlayers;
            private int _sensingRadius;

            private bool Enabled
            {
                get => _enabled;
                set
                {
                    _enabled = value;
                    if (Enabled)
                    {
                        Zone.BroadcastMessage(new SetStatusImmunityMessage
                        {
                            Associate = _spiderQueen,
                            ImmunityState = ImmunityState.Pop,
                            ImmuneToSpeed = true,
                            ImmuneToBasicAttack = true,
                            ImmuneToDOT = true
                        });
                
                        Zone.BroadcastMessage(new SetStunnedMessage
                        {
                            Associate = _spiderQueen,
                            StunState = StunState.Pop,
                            CantMove = true,
                            CantJump = true,
                            CantAttack = true,
                            CantTurn = true,
                            CantUseItem = true,
                            CantEquip = true,
                            CantInteract = true,
                            IgnoreImmunity = true
                        });
                        
                        _spiderQueen.GetComponent<DestroyableComponent>().Factions = _spiderQueenFactions;
                    }
                    else
                    {
                        _spiderQueen.GetComponent<DestroyableComponent>().Factions = new int[] {};
                        
                        Zone.BroadcastMessage(new SetStunnedMessage
                        {
                            Associate = _spiderQueen,
                            StunState = StunState.Push,
                            CantMove = true,
                            CantJump = true,
                            CantAttack = true,
                            CantTurn = true,
                            CantUseItem = true,
                            CantEquip = true,
                            CantInteract = true,
                            IgnoreImmunity = true
                        });

                        Zone.BroadcastMessage(new SetStatusImmunityMessage
                        {
                            Associate = _spiderQueen,
                            ImmunityState = ImmunityState.Push,
                            ImmuneToSpeed = true,
                            ImmuneToBasicAttack = true,
                            ImmuneToDOT = true
                        });
                    }
                }
            }


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
                    var impliedStage = newHealth < _stage3Threshold ? 3 
                        : newHealth < _stage2Threshold ? 2 : 1;
                    if (impliedStage > _stage)
                        WithdrawSpiderQueen();

                    if (newHealth <= 0)
                        await OnFightCompleted.InvokeAsync();
                });

                // Listen to smashed spiderlings to update the spiderling wave
                Listen(Zone.OnObject, o =>
                {
                    if (o is GameObject spiderling && spiderling.Lot == Lot.SpiderQueenSpiderling)
                    {
                        _spawnedSpiderlings.Add(spiderling);
                        spiderling.GetComponent<DestroyableComponent>().Factions = new int[] {4};
                        
                        Listen(spiderling.GetComponent<DestructibleComponent>().OnSmashed, 
                            (killer, lootOwner) => HandleSpiderlingDeath(spiderling));
                    }
                });

                foreach (var player in Zone.Players)
                {
                    Listen(player.OnFireServerEvent, (name, message) =>
                    {
                        if (message.Arguments == "CleanupSpiders")
                            CleanupSpiders();
                    });
                }
            }

            #endregion state

            #region ai
            #region withdrawal
            private void AdvanceSpiderQueen()
            {
                if (!_withdrawn)
                    return;

                _spiderQueen.Animate("advance");
                Zone.Schedule(AdvanceAttack, _advanceAnimationTime - 400);
                Zone.Schedule(AdvanceComplete, _advanceAnimationTime);

                _withdrawn = false;
            }
            
            private void AdvanceAttack()
            {
                if (_advanceAttackTarget != null)
                {
                    // TODO
                }
            }

            private void AdvanceComplete()
            {
                _stage += 1;
                _killedSpiders = 0;
                _currentSpiderlingWavecount = 0;
                _spiderQueen.Animate("taunt");
                Zone.Schedule(AdvanceTauntComplete, _advanceAnimationTime);
            }

            private void AdvanceTauntComplete()
            {
                Enabled = true;
            }
            
            private void WithdrawSpiderQueen()
            {
                if (_withdrawn)
                    return;

                // Orientation for the animation to make sense
                _spiderQueen.Transform.Rotate(new Quaternion { X = 0.0f, Y = -0.005077f, Z = 0.0f, W = 0.999f });
                _spiderQueen.Animate("withdraw");
                Enabled = false;

                Zone.Schedule(WithdrawalComplete, _withdrawalTime - 250);
                _withdrawn = true;
            }

            private void WithdrawalComplete()
            {
                _spiderQueen.Animate("idle-withdrawn");
                _currentSpiderlingWavecount = _stage == 1 ? _stage2SpiderlingCount : _stage3SpiderlingCount;
                _spiderEggsToPrep = _currentSpiderlingWavecount;
                SpawnSpiders();
            }
            #endregion withdrawal
            
            #region spiderwave
                        
            /// <summary>
            /// Removes all the spiders from the scene
            /// </summary>
            private void CleanupSpiders()
            {
                foreach (var spiderling in _spawnedSpiderlings)
                    Destroy(spiderling);
                _spawnedSpiderlings = new List<GameObject>();
            }

            private void SpawnSpiders()
            {
                var spiderEggs = _spiderEggSpawner.GetComponent<SpawnerComponent>().ActiveSpawns
                    .Except(_preppedSpiderEggs).ToList();
                
                // If no spider eggs are available, try again in a second
                if (spiderEggs.Count <= 0)
                {
                    Zone.Schedule(SpawnSpiders, 1000);
                    return;
                }
                
                var rng = new Random();
                var newlyPreppedEggs = 0;
                
                for (var i = 0; i < _spiderEggsToPrep; i++)
                {
                    var eggToPrep = spiderEggs[rng.Next(0, spiderEggs.Count)];
                    spiderEggs.Remove(eggToPrep);
                    _preppedSpiderEggs.Add(eggToPrep);
                    
                    Zone.BroadcastMessage(new PlayFXEffectMessage
                    {
                        Associate = eggToPrep,
                        EffectId = 2856,
                        EffectType = "maelstrom",
                        Name = "test"
                    });
                    
                    Zone.BroadcastMessage(new PlayFXEffectMessage
                    {
                        Associate = eggToPrep,
                        EffectId = 2260,
                        EffectType = "rebuild_medium",
                        Name = "dropdustmedium"
                    });

                    newlyPreppedEggs++;
                }
                
                _spiderEggsToPrep -= newlyPreppedEggs;
                
                // There weren't enough spider eggs to hatch, try again in a second
                if (_spiderEggsToPrep > 0)
                {
                    Zone.Schedule(SpawnSpiders, 1000);
                }
                else
                {
                    // Wait a bit to show the animation
                    Zone.Schedule(() =>
                    {
                        foreach (var eggToHatch in _preppedSpiderEggs)
                            SpawnSpiderling(eggToHatch);
                        _preppedSpiderEggs = new List<GameObject>();
                    }, 1500);
                }
            }

            private void SpawnSpiderling(GameObject egg)
            {
                var transform = egg.GetComponent<Transform>();
                egg.GetComponent<DestructibleComponent>().SmashAsync(_spiderQueen);
                        
                var spiderling = GameObject.Instantiate(new LevelObjectTemplate
                {
                    ObjectId = ObjectId.Standalone,
                    Lot = Lot.SpiderQueenSpiderling,
                    Position = transform.Position,
                    Rotation = transform.Rotation,
                    Scale = 1,
                    LegoInfo = new LegoDataDictionary()
                }, _spiderQueen);
                        
                Start(spiderling);
                GameObject.Construct(spiderling);
                GameObject.Serialize(spiderling);
                _spawnedSpiderlings.Add(spiderling);
            }
            
            private void HandleSpiderlingDeath(GameObject spiderling)
            {
                _killedSpiders++;
                _spawnedSpiderlings.Remove(spiderling);
                Destroy(spiderling);

                if (_killedSpiders >= _currentSpiderlingWavecount)
                    AdvanceSpiderQueen();
            }
            #endregion spiderwave
            #endregion ai
        }
    }
}