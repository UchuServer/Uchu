using System.Linq;
using Uchu.Core;
using Uchu.World.Collections;

namespace Uchu.World
{
    public class ModularBuilder : Component
    {
        private bool _building;

        public GameObject BasePlate;

        public ModularBuilder()
        {
            OnStart.AddListener(() =>
            {
                var inventory = GameObject.GetComponent<InventoryComponent>();

                inventory.OnEquipped.AddListener(item =>
                {
                    Logger.Information($"Equipped {item.ItemType} item");
                    if (item.ItemType == ItemType.LootModel)
                    {
                        StartBuildingWithItem(item);
                    }
                });
            });
        }
        
        public bool IsBuilding
        {
            get => _building;
            private set
            {
                As<Player>().Message(new SetStunnedMessage
                {
                    Associate = GameObject,
                    CantAttack = value
                });

                _building = value;

                var inventory = GameObject.GetComponent<InventoryManager>();

                if (value) return;
                foreach (var temp in inventory[InventoryType.TemporaryModels].Items)
                {
                    inventory.MoveItemsBetweenInventories(
                        temp,
                        temp.Lot,
                        temp.Count,
                        InventoryType.TemporaryModels,
                        InventoryType.Models
                    );
                }
            }
        }

        public void StartBuilding(StartBuildingWithItemMessage message)
        {
            As<Player>().SendChatMessage(
                $"[{IsBuilding}]\n{string.Join('\n', typeof(StartBuildingWithItemMessage).GetProperties().Select(p => $"{p.Name} = {p.GetValue(message)}"))}"
            );
            
            IsBuilding = true;
            
            BasePlate = message.Associate;
            
            As<Player>().Message(new StartArrangingWithItemMessage
            {
                Associate = GameObject,
                FirstTime = message.FirstTime,
                BuildArea = message.Associate,
                StartPosition = Transform.Position,
                
                SourceBag = message.SourceBag,
                Source = message.Source,
                SourceLot = message.SourceLot,
                SourceType = message.SourceType,
                
                Target = message.Target,
                TargetLot = message.TargetLot,
                TargetPosition = message.TargetPosition,
                TargetType = message.TargetType
            });
        }

        public void StartBuildingWithItem(Item item)
        {
            /*
            As<Player>().Message(new StartArrangingWithItemMessage
            {
                Associate = GameObject,
                FirstTime = false,
                BuildArea = BasePlate,
                StartPosition = Transform.Position,
                
                SourceBag = (int) item.Inventory.InventoryType,
                Source = item,
                SourceLot = item.Lot,
                SourceType = 8, // TODO: find out how to get this
                
                Target = null,
                TargetLot = BasePlate.Lot,
                TargetPosition = BasePlate.Transform.Position,
                TargetType = 0
            });
            */
        }
        
        public void FinishBuilding(Lot[] models)
        {
            var inventory = GameObject.GetComponent<InventoryManager>();

            foreach (var module in models)
            {
                inventory.RemoveItem(module, 1, InventoryType.TemporaryModels);
            }

            var model = new LegoDataDictionary
            {
                ["assemblyPartLOTs"] = LegoDataList.FromEnumerable(models.Select(s => s.Id))
            };

            inventory.AddItem(6416, 1, InventoryType.Models, model);
            
            var thinkingHat = inventory[InventoryType.Items].Items.First(i => i.Lot == 6086);
            
            GameObject.GetComponent<InventoryComponent>().UnEquipItem(thinkingHat);
            
            IsBuilding = false;
        }
        
        public void DoneArranging(DoneArrangingWithItemMessage message)
        {
            As<Player>().SendChatMessage($"DONE: {message.NewSource}\t{message.NewTarget}\t{message.OldSource}");
        }

        public void Pickup(Lot lot)
        {
            As<Player>().SendChatMessage($"PICKUP: {lot}");
            
            var inventory = GameObject.GetComponent<InventoryManager>();
            
            var item = inventory[InventoryType.TemporaryModels].Items.First(i => i.Lot == lot);
            
            GameObject.GetComponent<InventoryComponent>().EquipItem(item);
            
            /*
            As<Player>().Message(new StartArrangingWithItemMessage
            {
                Associate = GameObject,
                FirstTime = false
            });
            */
        }
    }
}