namespace Uchu.World
{
    public class RequestSmashPlayerMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestSmashPlayer;
    }
}