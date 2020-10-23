using System.Threading.Tasks;
using Uchu.Core;


namespace Uchu.World.Handlers.GameMessages
{
    public class PropertyHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task QueryPropertyDataHandler(QueryPropertyData message, Player player)
        {
            if (message.Associate.TryGetComponent<PropertyManagementComponent>(out var comp))
            {
                await comp.OnQueryPropertyData.InvokeAsync(message, player);
            } 
            else
            {
                message.Associate.AddComponent<PropertyManagementComponent>();
                await comp.OnQueryPropertyData.InvokeAsync(message, player);
            }
            
        }


    }
}
