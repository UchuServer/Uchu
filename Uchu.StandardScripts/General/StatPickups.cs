using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Client;
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
                    var objectSkill = (ClientCache.FindAll<ObjectSkills>(lot).Where(skill => skill.CastOnType == (int)SkillCastType.OnCollect)).FirstOrDefault();
                    if (objectSkill == default) return Task.CompletedTask;
                    var skillComponent = player.GetComponent<SkillComponent>();
                    var missionInventoryComponent = player.GetComponent<MissionInventoryComponent>();
                    skillComponent.CalculateSkillAsync((int) objectSkill.SkillID, player);
                    missionInventoryComponent.CollectPowerupAsync((int) objectSkill.SkillID);
                    return Task.CompletedTask;
                });

                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        }
    }
}