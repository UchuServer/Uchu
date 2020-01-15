using System.Linq;
using System.Numerics;
using Uchu.Core;

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