using RakDotNet.IO;

namespace Uchu.World
{
    public class DoneLoadingObjectsMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ServerDoneLoadingAllObjects;

        public override void SerializeMessage(BitWriter writer)
        {
        }
    }
}