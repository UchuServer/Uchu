using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    public class Coil : ObjectScript
    {
        //there isn't a script for this in the client, but three scripts have basically the exact same code, so it's easier to do this
        public int SkillID { get; set; }
        public int CoilThreshold { get; set; }
        private int CoilCount = 0;
        public Coil(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnStart, () =>
            {
                if (gameObject is Item item)
                {
                    Listen(item.Owner.GetComponent<InventoryComponent>().OnUnEquipped, unequippedItem =>
                    {
                        if (unequippedItem == item)
                        {
                            CoilCount = 0;
                            //remove recoil count when unequipped
                        }
                    });
                }
            });
        }
        protected void Process(Item item)
        {
            if (item.IsEquipped)
            {
                CoilCount++;
                if (CoilCount >= CoilThreshold)
                {
                    CoilCount = 0;
                    Effect(item.Owner);
                }
            }
            else
            {
                CoilCount = 0;
            }
        }
        private async void Effect(GameObject target)
        {
            Task.Delay(100);
            var skillComponent = target.GetComponent<SkillComponent>();
            skillComponent.CalculateSkillAsync(SkillID, target);
        }
    }
}