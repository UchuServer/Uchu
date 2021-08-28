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
        private int CoilCount { get; set; }
        public Coil(GameObject gameObject) : base(gameObject)
        {
            CoilCount = 0;
        }
        protected void Process(Item item)
        {
            if (item.IsEquipped)
            {
                CoilCount++;
                if (CoilCount >= CoilThreshold)
                {
                    CoilCount = 0;
                    var skillComponent = item.Owner.GetComponent<SkillComponent>();
                    skillComponent.CalculateSkillAsync(SkillID, item.Owner);
                }
            } 
            else 
            {
                CoilCount = 0;
            }
        }
    }
}