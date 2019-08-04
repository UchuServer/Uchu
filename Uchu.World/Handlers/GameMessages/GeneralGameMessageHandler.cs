using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class GeneralGameMessageHandler : HandlerGroup
    {
        [PacketHandler(RunTask = true)]
        public void ReadyForUpdateHandler(ReadyForUpdateMessage message, Player player)
        {
            Logger.Information($"Ready for update: {player.EndPoint} {message.GameObject}");
        }
    }
}