using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class PlayNDAudioEmitterMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlayNDAudioEmitter;
        public long NDAudioCallbackMessageData = 0;
        public long NDAudioEmitterID = 0;
        public string NDAudioEventGUID = "";
        public string NDAudioMetaEventName = "";
        public bool Result = false;
        ObjectId TargetObjectIDForNDAudioCallbackMessages = new ObjectId(0);
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(NDAudioCallbackMessageData != 0);
            if (NDAudioCallbackMessageData != 0)
            {
                writer.Write(NDAudioCallbackMessageData);
            }

            writer.Write(NDAudioEmitterID != 0);
            if (NDAudioEmitterID != 0)
            {
                writer.Write(NDAudioEmitterID);
            }

            writer.Write((uint)NDAudioEventGUID.Length);
            writer.WriteString(NDAudioEventGUID, NDAudioEventGUID.Length);

            writer.Write((uint)NDAudioMetaEventName.Length);
            writer.WriteString(NDAudioMetaEventName, NDAudioMetaEventName.Length);

            writer.WriteBit(Result);

            writer.Write(val: (ulong)TargetObjectIDForNDAudioCallbackMessages);
        }
    }
}