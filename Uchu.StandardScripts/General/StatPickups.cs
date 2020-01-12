using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    public class StatPickups : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnLootPickup, lot =>
                {
                    var stats = player.GetComponent<Stats>();
                    
                    switch (lot)
                    {
                        case Lot.Imagination:
                            stats.Imagination += 1;
                            break;
                        case Lot.TwoImagination:
                            stats.Imagination += 2;
                            break;
                        case Lot.ThreeImagination:
                            stats.Imagination += 3;
                            break;
                        case Lot.FiveImagination:
                            stats.Imagination += 5;
                            break;
                        case Lot.TenImagination:
                            stats.Imagination += 10;
                            break;
                        case Lot.Health:
                            stats.Health += 1;
                            break;
                        case Lot.TwoHealth:
                            stats.Health += 2;
                            break;
                        case Lot.ThreeHealth:
                            stats.Health += 3;
                            break;
                        case Lot.FiveHealth:
                            stats.Health += 5;
                            break;
                        case Lot.TenHealth:
                            stats.Health += 10;
                            break;
                        case Lot.Armor:
                            stats.Armor += 1;
                            break;
                        case Lot.TwoArmor:
                            stats.Armor += 2;
                            break;
                        case Lot.ThreeArmor:
                            stats.Armor += 3;
                            break;
                        case Lot.FiveArmor:
                            stats.Armor += 5;
                            break;
                        case Lot.TenArmor:
                            stats.Armor += 10;
                            break;
                        default:
                            return Task.CompletedTask;
                    }
                    
                    return Task.CompletedTask;
                });

                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        }
    }
}