using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    [ScriptName("imaginationbanknonfaction.lua")]
    public class ImaginationBankNonFaction : ObjectScript
    {
        public ImaginationBankNonFaction(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                bool Ready = true;
                Listen(item.Owner.GetComponent<DestroyableComponent>().OnImaginationChanged, (newI, delta) => 
                {
                    if (newI < 1 && Ready)
                    {
                        Task.Run(async () => 
                        {
                            Ready = false;
                            //prevent imagination from getting eaten by ability
                            await Task.Delay(100);
                            item.Owner.GetComponent<SkillComponent>().CalculateSkillAsync(394);
                            await Task.Delay(900);
                            Ready = true;
                        });
                    }
                });
            }
        }
    }
}