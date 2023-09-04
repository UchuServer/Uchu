using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class BuildBorderComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.BuildBorder;

        protected BuildBorderComponent()
        {
            Listen(OnStart, () => Listen(GameObject.OnInteract, OnInteract));
        }

        private void OnInteract(Player player)
        {
            
        }

        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}