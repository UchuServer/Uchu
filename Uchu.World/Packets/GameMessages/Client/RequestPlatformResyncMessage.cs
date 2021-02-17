namespace Uchu.World
{
    public class RequestPlatformResyncMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestPlatformResync;
    }
}