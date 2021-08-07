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
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnLootPickup, lot =>
                {
                    var stats = player.GetComponent<DestroyableComponent>();
                    var skill = player.GetComponent<SkillComponent>();
                    var missionInventoryComponent = player.GetComponent<MissionInventoryComponent>();
                    var skillExecute = 0;
                    switch (lot)
                    {
                        case Lot.Imagination:
                            //stats.Imagination += 1;
                            skillExecute = 13;
                            break;
                        case Lot.TwoImagination:
                            //stats.Imagination += 2;
                            skillExecute = 129;
                            break;
                        case Lot.ThreeImagination:
                            //stats.Imagination += 3;
                            skillExecute = 906;
                            break;
                        case Lot.FiveImagination:
                            //stats.Imagination += 5;
                            skillExecute = 907;
                            break;
                        case Lot.TenImagination:
                            //stats.Imagination += 10;
                            skillExecute = 908;
                            break;
                        case Lot.Health:
                            //stats.Health += 1;
                            skillExecute = 5;
                            break;
                        case Lot.TwoHealth:
                            //stats.Health += 2;
                            skillExecute = 902;
                            break;
                        case Lot.ThreeHealth:
                            //stats.Health += 3;
                            skillExecute = 903;
                            break;
                        case Lot.FiveHealth:
                            //stats.Health += 5;
                            skillExecute = 904;
                            break;
                        case Lot.TenHealth:
                            //stats.Health += 10;
                            skillExecute = 905;
                            break;
                        case Lot.Armor:
                            //stats.Armor += 1;
                            skillExecute = 747;
                            break;
                        case Lot.TwoArmor:
                            //stats.Armor += 2;
                            skillExecute = 909;
                            break;
                        case Lot.ThreeArmor:
                            //stats.Armor += 3;
                            skillExecute = 910;
                            break;
                        case Lot.FiveArmor:
                            //stats.Armor += 5;
                            skillExecute = 911;
                            break;
                        case Lot.TenArmor:
                            //stats.Armor += 10;
                            skillExecute = 912;
                            break;
                        default:
                            return Task.CompletedTask;
                    }
                    skill.CalculateSkillAsync(skillExecute, player);
                    missionInventoryComponent.StatPickupsAsync(skillExecute);
                    return Task.CompletedTask;
                });

                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        }
    }
}