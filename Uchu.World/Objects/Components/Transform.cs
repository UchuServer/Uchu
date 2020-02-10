using System;
using System.Linq;
using System.Numerics;

namespace Uchu.World
{
    public class Transform : Component
    {
        private Transform _parent;

        public Vector3 Position { get; set; } = Vector3.Zero;

        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        public Vector3 EulerAngles
        {
            get => Rotation.ToEuler();
            set => Rotation = Quaternion.CreateFromYawPitchRoll(value.X, value.Y, value.Z);
        }

        public void Translate(Vector3 delta)
        {
            Position += delta;

            GameObject.Serialize(GameObject);
        }
        
        public void Rotate(Quaternion delta)
        {
            Rotation += delta;

            GameObject.Serialize(GameObject);
        }

        public Vector3 Forward
        {
            get => Rotation.VectorMultiply(Vector3.UnitX);
            set => Rotation = value.QuaternionLookRotation(Vector3.UnitY);
        }

        public void LookAt(Vector3 position, bool lockY = true)
        {
            if (lockY)
            {
                position.Y = Position.Y;
            }

            // Determine which direction to rotate towards
            var targetDirection = position - Position;

            // Calculate a rotation a step closer to the target and applies rotation to this object
            Rotation = targetDirection.QuaternionLookRotation(Vector3.UnitY);

            GameObject.Serialize(GameObject);
        }
        
        public float Scale { get; set; } = -1;

        public Transform Parent
        {
            get => _parent;
            set
            {
                if (_parent == value) return;
                _parent = value;

                var list = _parent.Children.ToList();

                if (value == null)
                    list.Remove(this);
                else
                    list.Add(this);

                _parent.Children = list.ToArray();
            }
        }

        public Transform[] Children { get; private set; } = new Transform[0];
    }
}