using System;
using System.Numerics;

namespace Uchu.World.Experimental
{
    public class FloatingBrick : Component
    {
        private float MaxDistance;

        private float PassedTime;

        public float Speed;

        private Vector3 StartPos;
        public Vector3 Target;

        public FloatingBrick()
        {
            OnStart += () =>
            {
                StartPos = Transform.Position;
                MaxDistance = Vector3.Distance(Target, StartPos);
            };

            OnTick += Build;
        }

        private void Build()
        {
            Transform.Position = Vector3.Lerp(Transform.Position, Target, PassedTime / Speed);

            PassedTime += Zone.DeltaTime;

            Transform.Scale = Math.Clamp(PassedTime / Speed, 0, 1);

            if (PassedTime > Speed)
            {
                Destroy(GameObject);
                return;
            }

            GameObject.Serialize(GameObject);
        }
    }
}