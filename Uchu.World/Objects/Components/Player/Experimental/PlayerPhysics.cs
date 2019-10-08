namespace Uchu.World.Experimental
{
    public class PlayerPhysics : Component
    {
        private ControllablePhysicsComponent _physics;

        private Stats _stats;

        private float _prevY;

        public PlayerPhysics()
        {
            OnStart.AddListener(() =>
            {
                _physics = GameObject.GetComponent<ControllablePhysicsComponent>();
                _stats = GameObject.GetComponent<Stats>();
            });
            
            OnTick.AddListener(CheckFallDamage);
        }

        private void CheckFallDamage()
        {
            if ((_prevY < _physics.Velocity.Y || !_physics.HasVelocity) && _prevY < -55)
            {
                _stats.Damage(1, GameObject);

                var additionalDamage = -60;
                while (_prevY < additionalDamage)
                {
                    additionalDamage -= 5;
                    
                    _stats.Damage(1, GameObject);
                }
                
                _prevY = 0;
            }
            else _prevY = _physics.Velocity.Y;
        }
    }
}