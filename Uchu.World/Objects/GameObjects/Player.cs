using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Physics;
using Uchu.World.Filters;
using Uchu.World.Social;

namespace Uchu.World
{
    public sealed class Player : GameObject
    {
        private float _gravityScale = 1;
        
        private Player()
        {
            OnRespondToMission = new Event<int, GameObject, Lot>();

            OnFireServerEvent = new Event<string, FireServerEventMessage>();

            OnPositionUpdate = new Event<Vector3, Quaternion>();

            OnLootPickup = new Event<Lot>();
            
            OnWorldLoad = new Event();

            Lock = new SemaphoreSlim(1, 1);

            Listen(OnStart, async () =>
            {
                Connection.Disconnected += reason =>
                {
                    Connection = default;
                    
                    Destroy(this);
                    
                    return Task.CompletedTask;
                };

                Listen(OnPositionUpdate, UpdatePhysics);

                if (TryGetComponent<DestructibleComponent>(out var destructibleComponent))
                {
                    destructibleComponent.OnResurrect.AddListener(() => { GetComponent<DestroyableComponent>().Imagination = 6; });
                }
                
                await using var ctx = new UchuContext();
                
                var character = await ctx.Characters
                    .Include(c => c.UnlockedEmotes)
                    .FirstAsync(c => c.Id == Id);

                foreach (var unlockedEmote in character.UnlockedEmotes)
                {
                    await UnlockEmoteAsync(unlockedEmote.EmoteId);
                }

                Zone.Update(this, async () =>
                {
                    await Perspective.TickAsync();

                    await CheckBannedStatusAsync();
                }, 20 * 5);
            });
            
            Listen(OnDestroyed, () =>
            {
                OnFireServerEvent.Clear();
                OnLootPickup.Clear();
                OnWorldLoad.Clear();
                OnPositionUpdate.Clear();
            });
        }

        public Event<string, FireServerEventMessage> OnFireServerEvent { get; }

        public Event<int, GameObject, Lot> OnRespondToMission { get; }

        public Event<Lot> OnLootPickup { get; }
        
        public Event OnWorldLoad { get; }

        public Event<Vector3, Quaternion> OnPositionUpdate { get; }

        public IRakConnection Connection { get; private set; }

        public Perspective Perspective { get; private set; }
        
        public PlayerChatChannel ChatChannel { get; set; }

        public GuildGuiState GuildGuiState { get; set; }
        
        public string GuildInviteName { get; set; }
        
        private SemaphoreSlim Lock { get; }
        
        public int Ping => Connection.AveragePing;
        
        public override string Name
        {
            get => ObjectName;
            set
            {
                ObjectName = value;
                
                Zone.BroadcastMessage(new SetNameMessage
                {
                    Associate = this,
                    Name = value
                });
            }
        }

        /// <summary>
        ///    Negative offset for the SetCurrency message.
        /// </summary>
        /// <remarks>
        ///    Used when the client adds currency by itself. E.g, achievements.
        /// </remarks>
        public long HiddenCurrency { get; set; }

        public long Currency
        {
            get
            {
                using var ctx = new UchuContext();
                var character = ctx.Characters.First(c => c.Id == Id);

                return character.Currency;
            }
            set => Task.Run(async () => { await SetCurrencyAsync(value); });
        }

        public long EntitledCurrency { get; set; }

        public long UniverseScore
        {
            get
            {
                using var ctx = new UchuContext();
                var character = ctx.Characters.First(c => c.Id == Id);

                return character.UniverseScore;
            }
            set => Task.Run(async () => { await SetUniverseScoreAsync(value); });
        }

        public long Level
        {
            get
            {
                using var ctx = new UchuContext();
                var character = ctx.Characters.First(c => c.Id == Id);

                return character.Level;
            }
            set => Task.Run(async () => { await SetLevelAsync(value); });
        }

        public float GravityScale
        {
            get => _gravityScale;
            set
            {
                _gravityScale = Math.Clamp(value, 0, 2);
                
                Message(new SetGravityScaleMessage
                {
                    Associate = this,
                    Scale = _gravityScale
                });
            }
        }

        public async Task<Character> GetCharacterAsync()
        {
            await using var ctx = new UchuContext();
            
            return await ctx.Characters.FirstAsync(c => c.Id == Id);
        }

        public async Task<bool> GetFlagAsync(int flag)
        {
            await using var ctx = new UchuContext();

            return await ctx.Flags.AnyAsync(f => f.CharacterId == Id && f.Flag == flag);
        }

        public async Task SetFlagAsync(int flag, bool state)
        {
            await Lock.WaitAsync();
            
            await using var ctx = new UchuContext();

            var entry = await ctx.Flags.FirstOrDefaultAsync(
                f => f.CharacterId == Id && f.Flag == flag
            );

            if (state)
            {
                await GetComponent<MissionInventoryComponent>().FlagAsync(flag);
            }
            
            if (entry != default && !state)
            {
                ctx.Flags.Remove(entry);

                await ctx.SaveChangesAsync();
            }
            else if (entry == default && state)
            {
                await ctx.Flags.AddAsync(new CharacterFlag
                {
                    CharacterId = Id,
                    Flag = flag
                });

                await ctx.SaveChangesAsync();
            }

            Message(new NotifyClientFlagChangeMessage
            {
                Associate = this,
                Flag = state,
                FlagId = flag
            });
            
            Lock.Release();
        }

        private async Task CheckBannedStatusAsync()
        {
            await using var ctx = new UchuContext();

            var character = await ctx.Characters.FirstAsync(c => c.Id == Id);

            var user = await ctx.Users.FirstAsync(u => u.Id == character.UserId);

            if (!user.Banned) return;
                
            try
            {
                await Connection.CloseAsync();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public async Task<float[]> GetCollectedAsync()
        {
            await using var ctx = new UchuContext();
            await using var cdContext = new CdClientContext();

            var character = await ctx.Characters
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks)
                .ThenInclude(t => t.Values)
                .SingleOrDefaultAsync(c => c.Id == Id);
            
            var flagTaskIds = cdContext.MissionTasksTable
                .Where(t => t.TaskType == (int) MissionTaskType.Collect)
                .Select(t => t.Uid);

            // Get all the mission task values that correspond to flag values
            var flagValues = character.Missions
                .SelectMany(m => m.Tasks
                    .Where(t => flagTaskIds.Contains(t.TaskId))
                    .SelectMany(t => t.ValueArray())).ToArray();
            
            return flagValues;
        }

        internal static async Task<Player> ConstructAsync(Character character, IRakConnection connection, Zone zone)
        {
            //
            // Create base gameobject
            //
            
            var instance = Instantiate<Player>(
                zone,
                character.Name,
                zone.SpawnPosition,
                zone.SpawnRotation,
                1,
                character.Id,
                1
            );
            
            //
            // Setup layers
            //
            
            instance.Layer = StandardLayer.Player;
            
            var layer = StandardLayer.All;
            layer -= StandardLayer.Hidden;
            layer -= StandardLayer.Spawner;
            
            instance.Perspective = new Perspective(instance);

            var maskFilter = instance.Perspective.AddFilter<MaskFilter>();
            maskFilter.ViewMask = layer;

            instance.Perspective.AddFilter<RenderDistanceFilter>();
            instance.Perspective.AddFilter<FlagFilter>();
            instance.Perspective.AddFilter<ExcludeFilter>();
            
            //
            // Set connection
            //

            instance.Connection = connection;

            //
            // Add serialized components
            //
            
            var controllablePhysics = instance.AddComponent<ControllablePhysicsComponent>();
            instance.AddComponent<DestructibleComponent>();
            var stats = instance.GetComponent<DestroyableComponent>();
            var characterComponent = instance.AddComponent<CharacterComponent>();
            var inventory = instance.AddComponent<InventoryComponent>();
            
            instance.AddComponent<LuaScriptComponent>();
            instance.AddComponent<SkillComponent>();
            instance.AddComponent<RendererComponent>();
            instance.AddComponent<PossessableOccupantComponent>();
            
            controllablePhysics.HasPosition = true;
            stats.HasStats = true;
            characterComponent.Character = character;
            
            //
            // Equip items
            //

            await using (var ctx = new UchuContext())
            {
                var items = await ctx.InventoryItems.Where(
                    i => i.CharacterId == character.Id && i.IsEquipped
                ).ToArrayAsync();

                foreach (var item in items)
                {
                    if (item.ParentId != ObjectId.Invalid) continue;
                    
                    await inventory.EquipAsync(new EquippedItem
                    {
                        Id = item.Id,
                        Lot = item.Lot
                    });
                }
            }
            
            //
            // Server Components
            //
            
            instance.AddComponent<MissionInventoryComponent>();
            instance.AddComponent<InventoryManagerComponent>();
            instance.AddComponent<TeamPlayerComponent>();
            instance.AddComponent<ModularBuilderComponent>();
            
            //
            // Physics
            //

            var physics = instance.AddComponent<PhysicsComponent>();

            var box = CapsuleBody.Create(
                zone.Simulation,
                instance.Transform.Position,
                instance.Transform.Rotation,
                new Vector2(2, 4)
            );

            physics.SetPhysics(box);

            instance.Listen(physics.OnEnter, instance.OnEnterCollision);

            instance.Listen(physics.OnCollision, instance.OnStayCollision);
            
            instance.Listen(physics.OnLeave, instance.OnLeaveCollision);
            
            //
            // Register player gameobject in zone
            //
            
            Start(instance);
            Construct(instance);

            //
            // Register player as an active in zone
            //
            
            await zone.RegisterPlayer(instance);

            return instance;
        }

        private void OnStayCollision(PhysicsComponent other)
        {
            Logger.Information($"{this} stayed {other.GameObject}");
        }
        
        private void OnEnterCollision(PhysicsComponent other)
        {
            Logger.Information($"{this} entered {other.GameObject}");
        }
        
        private void OnLeaveCollision(PhysicsComponent other)
        {
            Logger.Information($"{this} left {other.GameObject}");
        }

        private void UpdatePhysics(Vector3 position, Quaternion rotation)
        {
            if (!TryGetComponent<PhysicsComponent>(out var physicsComponent)) return;

            if (!(physicsComponent.Physics is PhysicsBody physics)) return;

            physics.Position = Transform.Position;

            physics.Rotation = Transform.Rotation;

            var details = GetComponent<ControllablePhysicsComponent>();
            
            physics.AngularVelocity = details.HasAngularVelocity ? details.AngularVelocity : Vector3.Zero;

            physics.LinearVelocity = details.HasVelocity ? details.Velocity : Vector3.Zero;
        }

        public async Task UnlockEmoteAsync(int emoteId)
        {
            await using var ctx = new UchuContext();

            var character = await ctx.Characters
                .Include(c => c.UnlockedEmotes)
                .FirstAsync(c => c.Id == Id);

            if (character.UnlockedEmotes.All(u => u.EmoteId != emoteId))
            {
                character.UnlockedEmotes.Add(new UnlockedEmote
                {
                    EmoteId = emoteId
                });

                await ctx.SaveChangesAsync();
            }
            
            Message(new SetEmoteLockStateMessage
            {
                Associate = this,
                EmoteId = emoteId,
                Lock = false
            });
        }

        public void Teleport(Vector3 position)
        {
            Message(new TeleportMessage
            {
                Associate = this,
                Position = position
            });
        }

        internal void UpdateView()
        {
            foreach (var gameObject in Zone.Spawned)
            {
                var spawned = Perspective.LoadedObjects.ToArray().Contains(gameObject);

                var view = Perspective.View(gameObject);
                    
                if (spawned && !view)
                {
                    Zone.SendDestruction(gameObject, this);

                    continue;
                }

                if (!spawned && view)
                {
                    Zone.SendConstruction(gameObject, this);
                }
            }
        }
        
        public void TriggerViewUpdate(GameObject gameObject)
        {
            var spawned = Perspective.LoadedObjects.ToArray().Contains(gameObject);

            var view = Perspective.View(gameObject);
                    
            if (spawned && !view)
            {
                Zone.SendDestruction(gameObject, this);

                return;
            }

            if (!spawned && view)
            {
                Zone.SendConstruction(gameObject, this);
            }
        }

        public void SendChatMessage(string message, PlayerChatChannel channel = PlayerChatChannel.Debug, Player author = null, ChatChannel chatChannel = World.ChatChannel.Public)
        {
            if (channel > ChatChannel) return;
            
            Message(new ChatMessagePacket
            {
                Message = $"{message}\0",
                Sender = author,
                IsMythran = author?.GameMasterLevel > 0,
                Channel = chatChannel
            });
        }

        public void Message(ISerializable package)
        {
            if (Id == ObjectId.Invalid) return;
            
            Connection.Send(package);
        }

        public async Task<bool> SendToWorldAsync(InstanceInfo specification, ZoneId zoneId)
        {
            await using var ctx = new UchuContext();

            var character = await ctx.Characters.FirstAsync(c => c.Id == Id);

            character.LastZone = zoneId;

            await ctx.SaveChangesAsync();
            
            Message(new ServerRedirectionPacket
            {
                Port = (ushort) specification.Port,
                Address = Server.Host
            });

            return true;
        }
        
        public async Task<bool> SendToWorldAsync(ZoneId zoneId)
        {
            Logger.Information($"Requesting server for: {zoneId}");

            InstanceInfo server;

            try
            {
                server = await ServerHelper.RequestWorldServerAsync(Server, zoneId);
            }
            catch (Exception e)
            {
                Logger.Error(e);

                return false;
            }
            
            Logger.Information($"Yielded {server?.Port.ToString() ?? "<void>"} for {zoneId}");
            
            if (server == default)
            {
                return false;
            }

            if (Server.Port != server.Port) return await SendToWorldAsync(server, zoneId);
            
            Logger.Error("Could not send a player to the same port as it already has");

            return false;
        }

        private async Task SetCurrencyAsync(long currency)
        {
            await using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.FirstAsync(c => c.Id == Id);

                character.Currency = currency;
                character.TotalCurrencyCollected += currency;

                await ctx.SaveChangesAsync();
            }

            Message(new SetCurrencyMessage
            {
                Associate = this,
                Currency = currency - HiddenCurrency
            });
        }

        private async Task SetUniverseScoreAsync(long score)
        {
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();
            
            var character = await ctx.Characters.FirstAsync(c => c.Id == Id);

            character.UniverseScore = score;

            Message(new ModifyLegoScoreMessage
            {
                Associate = this,
                Score = character.UniverseScore - UniverseScore
            });

            await ctx.SaveChangesAsync();
        }

        private async Task SetLevelAsync(long level)
        {
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();
            
            var character = await ctx.Characters.FirstAsync(c => c.Id == Id);

            var lookup = await cdClient.LevelProgressionLookupTable.FirstOrDefaultAsync(l => l.Id == level);

            if (lookup == default)
            {
                Logger.Error($"Trying to set {this} level to a level that does not exist.");
                return;
            }

            character.Level = level;

            Debug.Assert(lookup.RequiredUScore != null, "lookup.RequiredUScore != null");

            character.UniverseScore = lookup.RequiredUScore.Value;

            Message(new ModifyLegoScoreMessage
            {
                Associate = this,
                Score = character.UniverseScore - UniverseScore
            });

            await ctx.SaveChangesAsync();
        }

        ~Player()
        {
            Logger.Information($"Player freed: {Id}");
        }
    }
}