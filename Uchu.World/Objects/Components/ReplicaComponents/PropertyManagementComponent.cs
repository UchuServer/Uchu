using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.World.Services;
using Uchu.World;
using System.Numerics;
using Uchu.Core.Client;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using RakDotNet.IO;

namespace Uchu.World
{
    public class PropertyManagementComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.PropertyManagementComponent;

        public Event<QueryPropertyData, Player> OnQueryPropertyData { get; } // Message, Origin

        public PropertyManagementComponent()
        {
            OnQueryPropertyData = new Event<QueryPropertyData, Player>();

            Listen(OnStart, () =>
            {
                Listen(GameObject.OnInteract, async player =>
                {
                    await OnInteract(player);
                });
            });
        }

        public override void Construct(BitWriter writer) { }

        public override void Serialize(BitWriter writer) { } 

        public async Task OnInteract(Player player)
        {
            
        }
    }
}