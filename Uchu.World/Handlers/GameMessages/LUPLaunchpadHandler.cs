using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class LUPLaunchpadHandler : HandlerGroup
    {
        [PacketHandler]
        public async void EnterProperty1Handler(EnterProperty1Message message, Player player)
        {
            if (message.Associate.TryGetComponent<LUPLaunchpadComponent>(out var lupLaunchpadComponent))
                lupLaunchpadComponent.ChoiceBoxResponse(player, message.Index);
        }
    }
}
