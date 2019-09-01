using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.CdClient;

namespace Uchu.World
{
    public class Player : GameObject
    {
        public IPEndPoint EndPoint { get; private set; }

        public long Currency
        {
            get
            {
                using (var ctx = new UchuContext())
                {
                    var character = ctx.Characters.First(c => c.CharacterId == ObjectId);

                    return character.Currency;
                }
            }
            set { Task.Run(async () => { await SetCurrency(value); }); }
        }

        public long EntitledCurrency { get; set; }
        
        public static Player Create(Character character, IPEndPoint endPoint, Zone zone)
        {
            var instance = Instantiate<Player>(
                zone,
                character.Name,
                zone.ZoneInfo.SpawnPosition,
                zone.ZoneInfo.SpawnRotation,
                character.CharacterId,
                1
            );

            instance.EndPoint = endPoint;

            zone.RequestConstruction(instance);

            var controllablePhysics = instance.AddComponent<ControllablePhysicsComponent>();

            controllablePhysics.HasPosition = true;

            instance.AddComponent<DestructibleComponent>();

            var stats = instance.GetComponent<StatsComponent>();

            stats.HasStats = true;
            stats.CurrentHealth = (uint) character.CurrentHealth;
            stats.CurrentArmor = (uint) character.CurrentArmor;
            stats.CurrentImagination = (uint) character.CurrentImagination;
            stats.MaxHealth = (uint) character.MaximumHealth;
            stats.MaxArmor = (uint) character.MaximumArmor;
            stats.MaxImagination = (uint) character.MaximumImagination;

            var characterComponent = instance.AddComponent<CharacterComponent>();

            characterComponent.Level = (uint) character.Level;
            characterComponent.Character = character;

            var inventory = instance.AddComponent<InventoryComponent>();

            //
            // Equip items
            //
            
            var equippedItems = new Dictionary<EquipLocation, InventoryItem>();

            using (var cdClient = new CdClientContext())
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

            instance.AddComponent<LuaScriptComponent>();
            instance.AddComponent<SkillComponent>();
            instance.AddComponent<RendererComponent>();
            instance.AddComponent<Component107>();

            instance.Construct();

            instance.AddComponent<QuestInventory>();
            instance.AddComponent<InventoryManager>();
            instance.AddComponent<TeamPlayer>();
            
            Logger.Debug($"Player \"{character.Name}\" has been constructed.");
            
            return instance;
        }

        public void Message(IGameMessage gameMessage)
        {
            Logger.Debug($"Send {gameMessage} to {EndPoint} from {gameMessage.Associate.ObjectId}");
            Server.Send(gameMessage, EndPoint);
        }

        private async Task SetCurrency(long currency)
        {
            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.FirstAsync(c => c.CharacterId == ObjectId);
                
                character.Currency = currency;
                character.TotalCurrencyCollected += currency;

                await ctx.SaveChangesAsync();
            }
            
            Message(new SetCurrencyMessage
            {
                Associate = this,
                Currency = currency
            });
        }
    }
}