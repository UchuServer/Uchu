using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Objects;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    //lol
    [ScriptName("assemblyengineer1.lua")]
    [ScriptName("assemblyengineer2.lua")]
    [ScriptName("assemblyengineer3.lua")]
    [ScriptName("assemblyinventor1.lua")]
    [ScriptName("assemblyinventor2.lua")]
    [ScriptName("assemblyinventor3.lua")]
    [ScriptName("assemblysummoner1.lua")]
    [ScriptName("assemblysummoner2.lua")]
    [ScriptName("assemblysummoner3.lua")]
    [ScriptName("sentinelknight1.lua")]
    [ScriptName("sentinelknight2.lua")]
    [ScriptName("sentinelknight3.lua")]
    [ScriptName("sentinelspaceranger1.lua")]
    [ScriptName("sentinelspaceranger2.lua")]
    [ScriptName("sentinelspaceranger3.lua")]
    [ScriptName("sentinelsamurai1.lua")]
    [ScriptName("sentinelsamurai2.lua")]
    [ScriptName("sentinelsamurai3.lua")]
    public class FactionEquipment : SkillSetTriggerTemplate
    {
        private bool Started = false;
        public FactionEquipment(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnStart, () =>
            {
                if (gameObject is Item item)
                {
                    Action imagination = () =>
                    {
                        Listen(item.Owner.GetComponent<DestroyableComponent>().OnImaginationChanged, (newI, delta) =>
                        {
                            if (newI < 1)
                            {
                                Process(item);
                            }
                        });
                    };
                    Action armor = () =>
                    {
                        Listen(item.Owner.GetComponent<DestroyableComponent>().OnArmorChanged, (newA, delta) =>
                        {
                            if (newA < 1)
                            {
                                Process(item);
                            }
                        });
                    };
                    var sets = new Dictionary<int, (int skillID, int itemsRequired, float cooldownTime, Action method)>
                    {
                        //Engineer
                        {2, (394, 4, 11, imagination)}, //1
                        {3, (581, 4, 11, imagination)}, //2
                        {4, (582, 4, 11, imagination)}, //3
                        //Knight
                        {7, (559, 4, 0, armor)}, //1
                        {8, (560, 4, 0, armor)}, //2
                        {9, (561, 4, 0, armor)}, //3
                        //Space Ranger
                        {10, (1101, 4, 0, armor)}, //1
                        {11, (1102, 4, 0, armor)}, //2
                        {12, (1103, 4, 0, armor)}, //3
                        //Samurai
                        {13, (562, 4, 0, armor)}, //1
                        {14, (563, 4, 0, armor)}, //2
                        {15, (564, 4, 0, armor)}, //3
                        //Inventor
                        {25, (394, 4, 11, imagination)}, //1
                        {26, (581, 4, 11, imagination)}, //2
                        {27, (582, 4, 11, imagination)}, //3
                        //Summoner
                        {28, (394, 4, 11, imagination)}, //1
                        {29, (581, 4, 11, imagination)}, //2
                        {30, (582, 4, 11, imagination)}, //3
                    };

                    var itemSet = -1;
                    var inventoryComponent = item.Owner.GetComponent<InventoryComponent>();

                    //i couldn't find an item's set in the item itself
                    var context = new Uchu.Core.Client.CdClientContext();
                    foreach (var set in context.ItemSetsTable) if (set.ItemIDs.Contains(item.Lot.ToString())) itemSet = (int)set.SetID;
                    if (itemSet == -1 || !sets.ContainsKey(itemSet)) return;

                    var currentSet = sets[itemSet];

                    SkillID = currentSet.skillID;
                    SetID = itemSet;
                    ItemsRequired = currentSet.itemsRequired;
                    CooldownTime = currentSet.cooldownTime;

                    currentSet.method();
                }
            });
        }
    }
}