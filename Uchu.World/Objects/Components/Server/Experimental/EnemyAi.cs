using System.Numerics;

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

            OnTick.AddListener(PathFind);
        }

        public bool FollowPlayer { get; set; }

        public Vector3 TargetLocation { get; set; }

        public float Speed { get; set; }

        public Vector3 FollowLocation { get; private set; }

        private void PathFind()
        {
            var targetLocation = TargetLocation;

            if (FollowPlayer)
            {
                Player target = default;

                foreach (var player in Zone.Players)
                {
                    if (ReferenceEquals(target, default))
                    {
                        target = player;
                        continue;
                    }

                    if (Vector3.Distance(target.Transform.Position, Transform.Position) >
                        Vector3.Distance(player.Transform.Position, Transform.Position)) target = player;
                }

                _baseCombatAi.Target = target;

                if (ReferenceEquals(target, default))
                {
                    _baseCombatAi.PerformingAction = false;
                    _baseCombatAi.Action = CombatAiAction.Idle;
                    return;
                }

                targetLocation = target.Transform.Position;

                Transform.Rotation = target.Transform.Rotation;
            }

            _baseCombatAi.PerformingAction = true;

            _baseCombatAi.Action = CombatAiAction.Attacking;

            _ticks++;

            Transform.Position = Transform.Position.MoveTowards(targetLocation, Speed * Zone.DeltaTime);

            _controllablePhysics.HasPosition = true;

            _controllablePhysics.Velocity = Vector3.Normalize(Transform.Position - targetLocation);

            FollowLocation = targetLocation;

            if (_ticks == 5)
            {
                Update(GameObject);

                _ticks = default;
            }
        }
    }
}