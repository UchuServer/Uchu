using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class BuffBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Buff;
        
        public int Life { get; set; }
        
        public int Armor { get; set; }
        
        public int Imagination { get; set; }
        
        public float RunSpeed { get; set; }
        
        public float AttackSpeed { get; set; }
        
        public float Brain { get; set; }
        
        public override async Task BuildAsync()
        {
            Life = await GetParameter<int>("life");
            Armor = await GetParameter<int>("armor");
            Imagination = await GetParameter<int>("imag");
            RunSpeed = await GetParameter<int>("run_speed");
            AttackSpeed = await GetParameter<int>("attack_speed");
            Brain = await GetParameter<int>("brain");
        }
    }
}