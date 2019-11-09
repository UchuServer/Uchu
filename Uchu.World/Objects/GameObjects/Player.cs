using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Social;

namespace Uchu.World
{
    public class Player : GameObject
    {
        private Player()
        {
            OnTick.AddListener(CheckDeathZone);
            
            OnStart.AddListener(() =>
            {
                Connection.Disconnected += reason =>
                {
                    Destroy(this);

                    Zone.ManagedPlayers.Remove(this);
                    
                    return Task.CompletedTask;
                };
            });
        }

        public IRakConnection Connection { get; private set; }

        public Perspective Perspective { get; private set; }

        public readonly AsyncEventDictionary<string, FireServerEventMessage> OnFireServerEvent =
            new AsyncEventDictionary<string, FireServerEventMessage>();

        public readonly AsyncEvent<Lot> OnLootPickup = new AsyncEvent<Lot>();
        
        public async Task<Character> GetCharacterAsync()
        {
            await using var ctx = new UchuContext();
                
            return await ctx.Characters.FirstAsync(c => c.CharacterId == ObjectId);
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
                var character = ctx.Characters.First(c => c.CharacterId == ObjectId);

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
                var character = ctx.Characters.First(c => c.CharacterId == ObjectId);

                return character.UniverseScore;
            }
            set => Task.Run(async () => { await SetUniverseScoreAsync(value); });
        }

        public long Level
        {
            get
            {
                using var ctx = new UchuContext();
                var character = ctx.Characters.First(c => c.CharacterId == ObjectId);

                return character.Level;
            }
            set => Task.Run(async () => { await SetLevelAsync(value); });
        }

        internal static async Task<Player> ConstructAsync(Character character, IRakConnection connection, Zone zone)
        {
            //
            // Create base gameobject
            //
            
            var instance = Instantiate<Player>(
                zone,
                character.Name,
                zone.ZoneInfo.SpawnPosition,
                zone.ZoneInfo.SpawnRotation,
                1,
                character.CharacterId,
                1
            );

            instance.Connection = connection;

            //
            // Add serialized components
            //
            
            var controllablePhysics = instance.AddComponent<ControllablePhysicsComponent>();
            instance.AddComponent<DestructibleComponent>();
            var stats = instance.GetComponent<StatsComponent>();
            var characterComponent = instance.AddComponent<CharacterComponent>();
            var inventory = instance.AddComponent<InventoryComponent>();
            
            instance.AddComponent<LuaScriptComponent>();
            instance.AddComponent<SkillComponent>();
            instance.AddComponent<RendererComponent>();
            instance.AddComponent<Component107>();
            
            controllablePhysics.HasPosition = true;
            stats.HasStats = true;
            characterComponent.Character = character;
            
            //
            // Equip items
            //
            
            var equippedItems = new Dictionary<EquipLocation, InventoryItem>();

            await using (var cdClient = new CdClientContext())
            {
                foreach (var item in character.Items.Where(i => i.IsEquipped))
                {
                    var cdClientObject = cdClient.ObjectsTable.FirstOrDefault(
                        o => o.Id == item.LOT
                    );

                    var itemRegistryEntry = cdClient.ComponentsRegistryTable.FirstOrDefault(
                        r => r.Id == item.LOT && r.Componenttype == 11
                    );

                    if (cdClientObject == default || itemRegistryEntry == default)
                    {
                        Logger.Error($"{item.LOT} is not a valid item");
                        continue;
                    }

                    var itemComponent = cdClient.ItemComponentTable.First(
                        i => i.Id == itemRegistryEntry.Componentid
                    );

                    equippedItems.Add(itemComponent.EquipLocation, item);
                }
            }
            
            inventory.Items = equippedItems;
            
            //
            // Register player gameobject in zone
            //
            
            Start(instance);
            Construct(instance);

            //
            // Server Components
            //
            
            instance.AddComponent<MissionInventoryComponent>();
            instance.AddComponent<InventoryManagerComponent>();
            instance.AddComponent<TeamPlayerComponent>();
            instance.AddComponent<ModularBuilderComponent>();

            //
            // Setup layers
            //
            
            var layer = World.Layer.All;
            layer -= World.Layer.Hidden;
            layer -= World.Layer.Spawner;

            instance.Perspective = new Perspective(instance, layer);
            instance.Layer = World.Layer.Player;

            //
            // Register player as an active in zone
            //
            
            await zone.RegisterPlayer(instance);

            return instance;
        }

        public void Teleport(Vector3 position)
        {
            Message(new TeleportMessage
            {
                Associate = this,
                Position = position
            });
        }

        public void SendChatMessage(string message)
        {
            Message(new ChatMessagePacket
            {
                Message = $"{message}\0"
            });
        }

        public void Message(ISerializable gameMessage)
        {
            Logger.Debug($"Sending {gameMessage} to {this}{(gameMessage is IGameMessage g ? $" from {g.Associate}" : "")}");

            Connection.Send(gameMessage);
        }

        public void SendToWorld(ZoneId zoneId)
        {
            using var ctx = new UchuContext();
            
            var character = ctx.Characters.First(c => c.CharacterId == ObjectId);

            character.LastZone = (int) zoneId;

            ctx.SaveChanges();
            
            var address = Connection.EndPoint.Address.ToString() == "127.0.0.1"
                ? "localhost"
                : Server.GetAddresses()[0].ToString();

            Server.RequestWorldServer(zoneId, port =>
            {
                if (Server.Port == port)
                {
                    Logger.Error("Could not send a player to the same port as it already has");
                    
                    return;
                }
                
                Message(new ServerRedirectionPacket
                {
                    Port = (ushort) port,
                    Address = address
                });
            });
        }

        private async Task SetCurrencyAsync(long currency)
        {
            await using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.FirstAsync(c => c.CharacterId == ObjectId);

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
            
            var character = await ctx.Characters.FirstAsync(c => c.CharacterId == ObjectId);

            character.UniverseScore = score;

            foreach (var levelProgressionLookup in cdClient.LevelProgressionLookupTable)
            {
                if (levelProgressionLookup.RequiredUScore > score) break;

                Debug.Assert(levelProgressionLookup.Id != null, "levelProgressionLookup.Id != null");

                character.Level = levelProgressionLookup.Id.Value;
            }

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
            
            var character = await ctx.Characters.FirstAsync(c => c.CharacterId == ObjectId);

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

        private void CheckDeathZone()
        {
            // TODO: Remove

            var smashable = GetComponent<DestructibleComponent>();

            if (smashable == null || !smashable.Alive) return;

            switch ((ZoneId) Zone.ZoneInfo.ZoneId)
            {
                case ZoneId.VentureExplorerCinematic:
                    break;
                case ZoneId.VentureExplorer:
                    if (Transform.Position.Y <= 560) smashable.Smash(this, this);
                    break;
                case ZoneId.ReturnToVentureExplorer:
                    break;
                case ZoneId.AvantGardens:
                    break;
                case ZoneId.AvantGardensSurvival:
                    break;
                case ZoneId.SpiderQueenBattle:
                    break;
                case ZoneId.BlockYard:
                    break;
                case ZoneId.AvantGrove:
                    break;
                case ZoneId.NimbusStation:
                    break;
                case ZoneId.PetCove:
                    break;
                case ZoneId.VertigoLoopRacetrack:
                    break;
                case ZoneId.BattleOfNimbusStation:
                    break;
                case ZoneId.NimbusRock:
                    break;
                case ZoneId.NimbusIsle:
                    break;
                case ZoneId.FrostBurgh:
                    break;
                case ZoneId.GnarledForest:
                    break;
                case ZoneId.CanyonCove:
                    break;
                case ZoneId.KeelhaulCanyon:
                    break;
                case ZoneId.ChanteyShantey:
                    break;
                case ZoneId.ForbiddenValley:
                    break;
                case ZoneId.ForbiddenValleyDragon:
                    break;
                case ZoneId.DragonmawChasm:
                    break;
                case ZoneId.RavenBluff:
                    break;
                case ZoneId.Starbase3001:
                    break;
                case ZoneId.DeepFreeze:
                    break;
                case ZoneId.RobotCity:
                    break;
                case ZoneId.MoonBase:
                    break;
                case ZoneId.Portabello:
                    break;
                case ZoneId.LegoClub:
                    break;
                case ZoneId.CruxPrime:
                    break;
                case ZoneId.NexusTower:
                    break;
                case ZoneId.Ninjago:
                    break;
                case ZoneId.FrakjawBattle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}