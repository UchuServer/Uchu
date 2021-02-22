using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World.Systems.Match;

namespace Uchu.World.Handlers.GameMessages
{
    public class MatchHandler : HandlerGroup
    {
        [PacketHandler]
        public void MatchRequestHandler(MatchRequestMessage message, Player player)
        {
            // TODO: Convert type to enum. Determines if it is joining (0) or updating if the player is ready (1)
            Provisioner.GetProvisioner(message.Type).AddPlayer(player);
        }
    }
}