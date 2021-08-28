using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/skillsettriggertemplate.lua
    /// </summary>
    [ScriptName("skillsettriggertemplate.lua")]
    public class SkillSetTriggerTemplate : ObjectScript
    {
        public int SkillID { get; set; }
        public int SetID { get; set; }
        //public bool Triggered { get; set; }
        public float CooldownTime { get; set; }
        public int ItemsRequired { get; set; }
        //private Item[] PreviousItems = {};
        //private bool Prioritize = true;
        //private int SetItemCount { get; set; }
        protected bool Ready = true;
        //these variables aren't synced between equipment with the same script but it shouldn't matter too much
        public SkillSetTriggerTemplate(GameObject gameObject) : base(gameObject)
        {
        }
        protected void Process(Item item)
        {
            if (item.IsEquipped && Ready)
            {
                bool priority = true;
                var inventoryComponent = item.Owner.GetComponent<InventoryComponent>();
                foreach (var otherItem in inventoryComponent.EquippedItems)
                {
                    if (otherItem.Lot > item.Lot && inventoryComponent.ActiveItemSets.Find(i => i.SetID == SetID).ItemsInSet.Contains(otherItem.Lot))
                    {
                        priority = false;
                    }
                }
                if (priority)
                {
                    Effect(item.Owner);
                }
            }
        }
        private async void Effect(GameObject target)
        {
            Ready = false;
            var skillComponent = target.GetComponent<SkillComponent>();
            Zone.Schedule(() => 
            {
                Ready = true;
            }, 
            CooldownTime * 1000);
            //for "get 1 imagination back", the ability would eat the gained imagination as well (sextant with engie 1),
            //this delay prevents it from eating the gained stats
            await Task.Delay(100);
            skillComponent.CalculateSkillAsync(SkillID, target);
            
            //await Task.Delay((int) (CooldownTime * 1000));
            //Ready = true;
        }
    }
}