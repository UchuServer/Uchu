using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct PetTamingTryBuildResultMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.PetTamingTryBuildResult;
        public bool Success { get; set; }
        [Default(0)]
        public int NumberCorrect { get; set; }
    }
}