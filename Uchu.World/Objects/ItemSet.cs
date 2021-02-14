using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World.Objects
{
    public class ItemSet : Object
    {
        private ItemSet(InventoryComponent inventory, int setId)
        {
            _setId = setId;
            _owner = inventory;
            _equippedItemsInSet = new HashSet<Lot>();
            _skillSetMap = new Dictionary<int, int?>();
            _itemsInSet = new HashSet<Lot>();
            _owner.ActiveItemSets.Add(this);

            Listen(_owner.OnEquipped, async item =>
            {
                if (_itemsInSet.Contains(item.Lot))
                {
                    _equippedItemsInSet.Add(item.Lot);
                    
                    // Equip the skill set if this amount of items should equip one
                    if (_skillSetMap.TryGetValue(_equippedItemsInSet.Count, out var skillSetId)
                        && skillSetId.HasValue
                        && _owner.GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
                        await skillComponent.EquipSkillSet(skillSetId.Value);
                }
            });

            Listen(_owner.OnUnEquipped, async item =>
            {
                if (_itemsInSet.Contains(item.Lot))
                {
                    if (_skillSetMap.TryGetValue(_equippedItemsInSet.Count, out var skillSetId)
                        && skillSetId.HasValue
                        && _owner.GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
                        await skillComponent.UnequipSkillSetAsync(skillSetId.Value);
                    
                    _equippedItemsInSet.Remove(item.Lot);
                }
                
                if (_equippedItemsInSet.Count == 0)
                    Destroy(this);
            });

            Listen(_owner.OnDestroyed, () =>
            {
                Destroy(this);
            });
        }
        
        private readonly InventoryComponent _owner;
        private readonly HashSet<Lot> _equippedItemsInSet;
        private readonly HashSet<Lot> _itemsInSet;
        private readonly Dictionary<int, int?> _skillSetMap;
        private readonly int _setId;
        
        public static async Task EnsureActiveForItem(InventoryComponent inventory, Lot lot)
        {
            var clientItemSet = (await ClientCache.GetTableAsync<ItemSets>()).FirstOrDefault(
                i => i.ItemIDs.Contains(lot.ToString()));
            
            // Make sure that the item set is valid and that the inventory doesn't already track an item set with this ID
            if (clientItemSet?.SetID == null 
                || inventory.ActiveItemSets.Any(i => i._setId == clientItemSet.SetID.Value))
                return;
            
            var itemSet = new ItemSet(inventory, clientItemSet.SetID.Value);
            foreach (var itemSetLot in clientItemSet.ItemIDs.Split(","))
            {
                itemSet._itemsInSet.Add(new Lot(int.Parse(itemSetLot)));
            }
            
            // The skill sets that are unlocked when wearing n items of a set
            itemSet._skillSetMap[2] = clientItemSet.SkillSetWith2;
            itemSet._skillSetMap[3] = clientItemSet.SkillSetWith3;
            itemSet._skillSetMap[4] = clientItemSet.SkillSetWith4;
            itemSet._skillSetMap[5] = clientItemSet.SkillSetWith5;
            itemSet._skillSetMap[6] = clientItemSet.SkillSetWith6;
        }
    }
}