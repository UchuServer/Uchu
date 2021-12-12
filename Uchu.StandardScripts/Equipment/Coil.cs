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
        protected bool Ready = false;
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
                    Listen(item.Owner.GetComponent<DestroyableComponent>().OnHealthChanged, (newH, delta) =>
                    {
                        if (delta < 0)
                        {
                            Process(item);
                        }
                    });
                    Listen(item.Owner.GetComponent<DestroyableComponent>().OnArmorChanged, (newA, delta) =>
                    {
                        if (delta < 0)
                        {
                            Process(item);
                        }
                    });
                }
            });
        }
        protected void Process(Item item)
        {
            if (item.IsEquipped && Ready)
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
            //TODO: remove delay
            Task.Delay(100);
            var skillComponent = target.GetComponent<SkillComponent>();
            skillComponent.CalculateSkillAsync(SkillID, target);
        }
    }
}