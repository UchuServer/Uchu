using System;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Experimental
{
    public class PainElemental : Component
    {
        private EnemyAi _ai;

        private BaseCombatAiComponent _baseCombatAi;

        private readonly Random _random;

        private float _soulTimer;

        public PainElemental()
        {
            _random = new Random();

            OnStart += () =>
            {
                _ai = GameObject.GetComponent<EnemyAi>();
                _baseCombatAi = GameObject.GetComponent<BaseCombatAiComponent>();

                _ai.Speed = 15;
                _ai.FallowPlayer = true;
            };

            OnTick += Tick;
        }

        private void Tick()
        {
            if (ReferenceEquals(_baseCombatAi.Target, null))
            {
                _ai.Speed = default;
                return;
            }

            _ai.Speed = Vector3.Distance(Transform.Position, _baseCombatAi.Target.Transform.Position) >= 30 ? 15 : 0;

            if (_soulTimer < 3)
            {
                _soulTimer += Zone.DeltaTime;
                return;
            }

            _soulTimer = default;

            if (Vector3.Distance(Transform.Position, _ai.FallowLocation) < 120)
                for (var i = 0; i < 1; i++)
                {
                    var lostSoul = GameObject.Instantiate(Zone, 12379, Transform.Position, Transform.Rotation);

                    var lostSoulAi = lostSoul.GetComponent<EnemyAi>();

                    lostSoulAi.TargetLocation = _baseCombatAi.Transform.Position + new Vector3
                    {
                        X = (float) _random.NextDouble() * 3,
                        Y = (float) _random.NextDouble() * 3,
                        Z = (float) _random.NextDouble() * 3
                    };

                    lostSoulAi.Speed = 90;
                    lostSoulAi.FallowPlayer = true;

                    Update(GameObject);

                    Task.Run(async () =>
                    {
                        await Task.Delay(2000);

                        Logger.Debug("Destroying Soul");
                        Destroy(lostSoulAi.GameObject);
                    });
                }
        }
    }
}