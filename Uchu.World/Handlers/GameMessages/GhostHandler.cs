using Uchu.Core;
using Uchu.World.Filters;

namespace Uchu.World.Handlers.GameMessages
{
    public class GhostHandler : HandlerGroup
    {
        [PacketHandler]
        public void ToggleGhostReferenceOverrideHandler(ToggleGhostReferenceOverrideMessage message, Player player)
        {
            if (!player.Perspective.TryGetFilter<RenderDistanceFilter>(out var filter)) return;
            
            player.SendChatMessage($"Override: {message.Override}");
                
            filter.Override = message.Override;
        }

        [PacketHandler]
        public void SetGhostReferencePositionHandler(SetGhostReferencePositionMessage message, Player player)
        {
            if (!player.Perspective.TryGetFilter<RenderDistanceFilter>(out var filter)) return;
            
            player.SendChatMessage($"Override position: {message.Position}");
                
            filter.OverrideReferencePosition = message.Position;
        }
    }
}