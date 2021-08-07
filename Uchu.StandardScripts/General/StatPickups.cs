using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    public class StatPickups : NativeScript
    {
        public override Task LoadAsync()
        {
            var pickups = new Dictionary<int, int>()
            {
                {Lot.Imagination, 13},
                {Lot.TwoImagination, 129},
                {Lot.ThreeImagination, 906},
                {Lot.FiveImagination, 907},
                {Lot.TenImagination, 908},
                {Lot.Health, 5},
                {Lot.TwoHealth, 902},
                {Lot.ThreeHealth, 903},
                {Lot.FiveHealth, 904},
                {Lot.TenHealth, 905},
                {Lot.Armor, 747},
                {Lot.TwoArmor, 909},
                {Lot.ThreeArmor, 910},
                {Lot.FiveArmor, 911},
                {Lot.TenArmor, 912},
            };
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnLootPickup, lot =>
                {
                    if (!pickups.ContainsKey(lot)) return Task.CompletedTask;
                    var stats = player.GetComponent<DestroyableComponent>();
                    var skill = player.GetComponent<SkillComponent>();
                    var missionInventoryComponent = player.GetComponent<MissionInventoryComponent>();
                    skill.CalculateSkillAsync(pickups[lot], player);
                    missionInventoryComponent.StatPickupsAsync(pickups[lot]);
                    return Task.CompletedTask;
                });

                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        }
    }
}