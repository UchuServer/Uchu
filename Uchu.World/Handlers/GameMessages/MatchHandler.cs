using Uchu.Core;
using Uchu.World.Systems.Match;

namespace Uchu.World.Handlers.GameMessages
{
    public class MatchHandler : HandlerGroup
    {
        [PacketHandler]
        public void MatchRequestHandler(MatchRequestMessage message, Player player)
        {
            if (message.Type == MatchRequestType.Join)
            {
                // Add the player to a match.
                Provisioner.GetProvisioner(message.Value).AddPlayer(player);
            }
            else if (message.Type == MatchRequestType.SetReady)
            {
                // Set the player as ready or not.
                if (message.Value == 1)
                {
                    Provisioner.PlayerReady(player);
                } else if (message.Value == 0)
                {
                    Provisioner.PlayerNotReady(player);
                }
            }
        }
    }
}