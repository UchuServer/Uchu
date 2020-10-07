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
        public float Duration { get; set; } = -1;
        public int IconID { get; set; } = -1;
        public string MainText { get; set; }
        public string MixerProgram { get; set; }
        public string MusicCue { get; set; }
        public string PathNodeName { get; set; }
        public string SoundGUID { get; set; }
        public string SubText { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(Animation.Length);
            writer.WriteString(Animation, Animation.Length, true);

            writer.WriteBit(BackgroundObject != 11164);
            if (BackgroundObject != 11164) writer.Write(BackgroundObject.Id);

            writer.WriteBit(CameraPathLOT != 12458);
            if (CameraPathLOT != 11164) writer.Write(CameraPathLOT.Id);

            writer.WriteBit(CeleLeadIn != 1.0f);
            if (CeleLeadIn != 1.0f) writer.Write(CeleLeadIn);

            writer.WriteBit(CeleLeadOut != 0.8f);
            if (CeleLeadOut != 0.8f) writer.Write(CeleLeadOut);

            writer.WriteBit(CelebrationID != -1);
            if (CelebrationID != -1) writer.Write(CelebrationID);

            writer.Write(Duration);

            writer.Write(IconID);

            MainText ??= "";
            MixerProgram ??= "";
            MusicCue ??= "";
            SoundGUID ??= "";
            SubText ??= "";

            writer.Write(MainText.Length);
            writer.WriteString(MainText, MainText.Length, true);

            writer.Write(MixerProgram.Length);
            writer.WriteString(MixerProgram, MixerProgram.Length);

            writer.Write(MusicCue.Length);
            writer.WriteString(MusicCue, MusicCue.Length);

            writer.Write(PathNodeName.Length);
            writer.WriteString(PathNodeName, PathNodeName.Length);

            writer.Write(SoundGUID.Length);
            writer.WriteString(SoundGUID, SoundGUID.Length);

            writer.Write(SubText.Length);
            writer.WriteString(SubText, SubText.Length, true);
        }
    }
}