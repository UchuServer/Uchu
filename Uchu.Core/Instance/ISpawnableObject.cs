using System.Net;
using System.Numerics;

namespace Uchu.Core
{
    public interface ISpawnableObject<T> : ISpawnable
        where T : IPhysics
    {
        T Physics { get; }

        void UpdatePosition(Vector3 position);

        void UpdatePosition(T physics);
    }
}