using System.Numerics;

namespace Uchu.Core
{
    public interface IPhysics : IReplicaComponent
    {
        bool HasPosition { get; set; }
        Vector3 Position { get; set; }
        Vector4 Rotation { get; set; }
    }
}