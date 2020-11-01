using RakDotNet.IO;
using Uchu.Core;
using Uchu.World;

namespace Uchu.World
{
    public class SetStatusImmunityMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetStatusImmunity;

        public ImmunityState state;
        public bool bImmuneToBasicAttack = false;
        public bool bImmuneToDOT = false;
        public bool bImmuneToImaginationGain = false;
        public bool bImmuneToImaginationLoss = false;
        public bool bImmuneToInterrupt = false;
        public bool bImmuneToKnockback = false;
        public bool bImmuneToPullToPoint = false;
        public bool bImmuneToQuickbuildInterrupt = false;
        public bool bImmuneToSpeed = false;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((int)state);
            writer.WriteBit(bImmuneToBasicAttack);
            writer.WriteBit(bImmuneToDOT);
            writer.WriteBit(bImmuneToImaginationGain);
            writer.WriteBit(bImmuneToImaginationLoss);
            writer.WriteBit(bImmuneToInterrupt);
            writer.WriteBit(bImmuneToKnockback);
            writer.WriteBit(bImmuneToPullToPoint);
            writer.WriteBit(bImmuneToQuickbuildInterrupt);
            writer.WriteBit(bImmuneToSpeed);
        }
    }
}