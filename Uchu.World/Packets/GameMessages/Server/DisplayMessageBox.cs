using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct DisplayMessageBox
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.DisplayMessageBox;
        public bool Show { get; set; }
        public GameObject ClientCallback { get; set; }
        [Wide]
        public string Id { get; set; }
        public int ImageId { get; set; }
        [Wide]
        public string Text { get; set; }
        [Wide]
        public string UserData { get; set; }
    }
}