using Uchu.Core;

namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct BuybackFromVendorMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.BuybackFromVendor;
        public bool Confirmed { get; set; }
        [Default(1)]
        public int Count { get; set; }
        public Item Item { get; set; }

        public BuybackFromVendorMessage(GameObject associate = default, bool confirmed = default, int count = 1, Item item = default)
        {
            this.Associate = associate;
            this.Confirmed = confirmed;
            this.Count = count;
            this.Item = item;
        }
    }
}