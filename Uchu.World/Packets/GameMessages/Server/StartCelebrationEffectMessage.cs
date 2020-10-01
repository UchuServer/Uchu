using RakDotNet.IO;
using System.Reflection.Metadata;
using Uchu.Core;

namespace Uchu.World
{
    public class StartCelebrationEffectMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.StartCelebrationEffect;
        public string Animation { get; set; }
        public Lot BackgroundObject { get; set; } = 11164;
        public Lot CameraPathLOT { get; set; } = 12458;
        public float CeleLeadIn { get; set; } = 1.0f;
        public float CeleLeadOut { get; set; } = 0.8f;
        public int CelebrationID { get; set; } = -1;
        public float Duration { get; set; }
        public int IconID { get; set; }
        public string MainText { get; set; }
        public string MixerProgram { get; set; }
        public string MusicCue { get; set; }
        public string PathNodeName { get; set; }
        public string SoundGUID { get; set; }
        public string SubText { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            // This is how DLU handles this GM and it works so who cares.

            writer.Write<uint>(0);
            writer.WriteBit(false);
            writer.WriteBit(false);
            writer.WriteBit(false);
            writer.WriteBit(false);
            writer.WriteBit(true);
            writer.Write(CelebrationID);
            writer.Write(0.0f);
            writer.Write<uint>(0);
            writer.Write<uint>(0);
            writer.Write<uint>(0);
            writer.Write<uint>(0);
            writer.Write<uint>(0);
            writer.Write<uint>(0);
            writer.Write<uint>(0);
        }
    }
}