using RakDotNet.IO;
using Uchu.Core;
using Uchu.World;

namespace Uchu.World
{
    public class SetStatusImmunityMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetStatusImmunity;

        public ImmunityState ImmunityState;
        public bool ImmuneToBasicAttack { get; set; }
        public bool ImmuneToDOT { get; set; }
        public bool ImmuneToImaginationGain { get; set; }
        public bool ImmuneToImaginationLoss { get; set; }
        public bool ImmuneToInterrupt { get; set; }
        public bool ImmuneToKnockback { get; set; }
        public bool ImmuneToPullToPoint { get; set; }
        public bool ImmuneToQuickbuildInterrupt { get; set; }
        public bool ImmuneToSpeed { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((int)ImmunityState);
            writer.WriteBit(ImmuneToBasicAttack);
            writer.WriteBit(ImmuneToDOT);
            writer.WriteBit(ImmuneToImaginationGain);
            writer.WriteBit(ImmuneToImaginationLoss);
            writer.WriteBit(ImmuneToInterrupt);
            writer.WriteBit(ImmuneToKnockback);
            writer.WriteBit(ImmuneToPullToPoint);
            writer.WriteBit(ImmuneToQuickbuildInterrupt);
            writer.WriteBit(ImmuneToSpeed);
        }
    }
}