using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class NotifyRewardMailed : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.NotifyRewardMailed;

        public ObjectId ObjectId;

        public Vector3 StartPoint;

        public ObjectId Subkey = (ObjectId) (-1);

        public Lot TemplateId;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(ObjectId);

            writer.Write(StartPoint);

            writer.Write(Subkey);

            writer.Write(TemplateId);
        }
    }
}