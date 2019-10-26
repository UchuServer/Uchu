using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World;
using Uchu.World.Scripting;
using InventoryComponent = Uchu.World.InventoryComponent;

namespace StandardScripts.General
{
    public class GearStats : Script
    {
        public override Task LoadAsync()
        {
            Zone.OnPlayerLoad.AddListener(async player =>
            {
                var inventory = player.GetComponent<InventoryComponent>();
                
                inventory.OnEquipped.AddListener(_ => CalculateGearStats(player));
                inventory.OnUnEquipped.AddListener(_ => CalculateGearStats(player));

                await CalculateGearStats(player);
            });
            
            return Task.CompletedTask;
        }

        private static async Task CalculateGearStats(Player player)
        {
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();

            var character = await ctx.Characters.FirstOrDefaultAsync(c => c.CharacterId == player.ObjectId);
            
            if (character == default) return;
            
            var health = character.MaximumHealth = character.BaseHealth;
            var armor = character.MaximumArmor;
            var imagination = character.MaximumImagination = character.BaseImagination;
            
            var inventory = player.GetComponent<InventoryComponent>();

            foreach (var item in inventory.Items.Values)
            {
                var itemComponent = await cdClient.ItemComponentTable.FirstOrDefaultAsync(i =>
                    i.Id == ((Lot) item.LOT).GetComponentId(ComponentId.ItemComponent)
                );
                
                var objectSkills = cdClient.ObjectSkillsTable.Where(i =>
                    i.ObjectTemplate == (Lot) item.LOT
                );

                foreach (var objectSkill in objectSkills)
                {
                    if (objectSkill.CastOnType != (int) SkillCastType.OnEquip) continue;
                    
                    var skill = await cdClient.SkillBehaviorTable.FirstOrDefaultAsync(s =>
                        s.SkillID == objectSkill.SkillID);
                }
            }
        }
    }
}