using RakDotNet;

namespace Uchu.Core
{
    public interface IReplicaComponent : ISerializable
    {
        void Construct(BitStream stream);
    }
}