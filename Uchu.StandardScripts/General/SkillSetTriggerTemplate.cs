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
        public bool Triggered { get; set; }
        public float CooldownTime { get; set; }
        public int ItemsRequired { get; set; }
        private Item[] PreviousItems = {};
        private bool Prioritize = true;
        private int SetItemCount { get; set; }
        protected bool Ready = true;
        //these variables aren't synced between equipment with the same script but it shouldn't matter too much
        public SkillSetTriggerTemplate(GameObject gameObject) : base(gameObject)
        {
        }
        protected void Process(Item item)
        {
            //this is ugly as sin, but since there's up to a total of 6 items on the player at once with this script, this has to be done to make sure it doesnt fire 6 times, only once
            
            //if the item isn't equipped, or we aren't ready, don't run this
            if (item.IsEquipped && Ready){
                var inventoryComponent = item.Owner.GetComponent<InventoryComponent>();
                //save on checking item priority if we have the same stuff as last time
                if (PreviousItems == inventoryComponent.EquippedItems)
                {
                    //if we have priority, and we have the threshold amount of set items, fire
                    if (Prioritize && SetItemCount >= ItemsRequired)
                    {
                        Effect(item.Owner);
                    }
                }
                else
                {
                    Prioritize = true;
                    SetItemCount = 0;
                    //check each other item currently worn
                    foreach (Item otherItem in inventoryComponent.EquippedItems)
                    {
                        //if the other item doesn't have a script, continue
                        //if we lost priority, we also continue to not waste time
                        if (otherItem.TryGetComponent<LuaScriptComponent>(out var otherScript) && Prioritize)
                        {
                            //if the other item's script isn't ours, continue
                            if (item.GetComponent<LuaScriptComponent>().ScriptName == otherScript.ScriptName)
                            {
                                //if both have the same script, then it's part of the same set (proxy items have this script attached as well)
                                //increase the set counter
                                SetItemCount++;
                                if (item.Lot < otherItem.Lot)
                                {
                                    //prioritize the script with the higher LOT
                                    Prioritize = false;
                                }
                            }
                        }
                    }
                    //set the previous items so that we potentially don't have to do this mess again
                    PreviousItems = inventoryComponent.EquippedItems;
                    //same check as before
                    if (Prioritize && SetItemCount >= ItemsRequired)
                    {
                        Effect(item.Owner);
                    }
                }
            }
        }
        private async void Effect(GameObject target)
        {
            Ready = false;
            //await Task.Delay(100);
            target.GetComponent<SkillComponent>().CalculateSkillAsync(SkillID, target);
            await Task.Delay((int) (CooldownTime * 1000));
            Ready = true;
        }
    }
}