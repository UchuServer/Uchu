namespace Uchu.World
{
    public class RequestResurrectMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestResurrect;
    }
}