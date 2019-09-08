namespace Uchu.World
{
    public class RequestSmashPlayer : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestSmashPlayer;
    }
}