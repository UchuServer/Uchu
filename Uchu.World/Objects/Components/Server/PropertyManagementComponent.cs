using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.World.Services;
using Uchu.World;

namespace Uchu.World
{
    public class PropertyManagementComponent : Component
    {
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

            Listen(OnQueryPropertyData, (message, origin) =>
            {
                origin.Message(new DownloadPropertyDataMessage
                {
                    Associate = message.Associate,

                });
            });
        }

        public async Task OnInteract(Player player)
        {
            
        }
    }
}