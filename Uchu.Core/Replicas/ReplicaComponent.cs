using System;
using RakDotNet;

namespace Uchu.Core
{
    public abstract class ReplicaComponent  : IReplicaComponent
    {
        public abstract void Serialize(BitStream stream);

        public abstract void Construct(BitStream stream);

        public void Deserialize(BitStream stream)
        {
            throw new NotSupportedException();
        }
    }
}