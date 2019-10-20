namespace Uchu.World.Experimental
{
    public class EnemyAi : Component
    {
        private BaseCombatAiComponent _baseCombatAi;
        private ControllablePhysicsComponent _controllablePhysics;

        private int _ticks;

        public EnemyAi()
        {
            OnStart.AddListener(() =>
            {
                _baseCombatAi = GameObject.GetComponent<BaseCombatAiComponent>();
                _controllablePhysics = GameObject.GetComponent<ControllablePhysicsComponent>();
            });

            //OnTick.AddListener(PathFind);
        }

        private void PathFind()
        {
            _baseCombatAi.Action = CombatAiAction.Idle;

            if (_ticks != 5)
            {
                _ticks++;
                
                return;
            }
            
            GameObject.Serialize(GameObject);
            
            _ticks = default;
        }
    }
}