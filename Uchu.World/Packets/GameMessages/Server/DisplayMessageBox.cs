using RakDotNet;
using RakDotNet.IO;

namespace Uchu.World
{
    public class DisplayMessageBox : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.DisplayMessageBox;

        public bool Show { get; set; }

        public GameObject ClientCallback { get; set; }

        public string Id { get; set; }

        public int ImageId { get; set; }

        public string Text { get; set; }

        public string UserData { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(Show);

            writer.Write(ClientCallback);

            writer.Write((uint) Id.Length);
            writer.WriteString(Id, Id.Length, true);

            writer.Write(ImageId);

            writer.Write((uint) Text.Length);
            writer.WriteString(Text, Text.Length, true);

            writer.Write((uint) UserData.Length);
            writer.WriteString(UserData, UserData.Length, true);
        }
    }
}