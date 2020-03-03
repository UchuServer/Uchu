namespace Uchu.World
{
    public class NotifyServerLevelProcessingCompleteMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.NotifyServerLevelProcessingComplete;
    }
}