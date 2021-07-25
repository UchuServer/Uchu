using System;
using RakDotNet.IO;

namespace Uchu.World
{
    public class AddBuffMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.AddBuff;

        public bool AddedByTeammate { get; set; }

        public bool ApplyOnTeammates { get; set; }

        public bool CancelOnDamageAbsorbRanOut { get; set; }

        public bool CancelOnDamaged { get; set; }

        public bool CancelOnDeath { get; set; } = true; //default in docs

        public bool CancelOnLogout { get; set; }

        public bool CancelOnMove { get; set; }

        public bool CancelOnRemoveBuff { get; set; } = true; //default in docs, also ????

        public bool CancelOnUi { get; set; }

        public bool CancelOnUnequip { get; set; }

        public bool CancelOnZone { get; set; }

        public bool IgnoreImmunities { get; set; }

        public bool IsImmunity { get; set; }

        public bool UseRefCount { get; set; }
        
        public GameObject Caster { get; set; }

        public GameObject AddedBy { get; set; } // ????????

        public uint BuffID { get; set; }

        public uint DurationMS { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(AddedByTeammate);
            writer.WriteBit(ApplyOnTeammates);
            writer.WriteBit(CancelOnDamageAbsorbRanOut);
            writer.WriteBit(CancelOnDamaged);
            writer.WriteBit(CancelOnDeath);
            writer.WriteBit(CancelOnLogout);
            writer.WriteBit(CancelOnMove);
            writer.WriteBit(CancelOnRemoveBuff);
            writer.WriteBit(CancelOnUi);
            writer.WriteBit(CancelOnUnequip);
            writer.WriteBit(CancelOnZone);
            writer.WriteBit(IgnoreImmunities);
            writer.WriteBit(IsImmunity);
            writer.WriteBit(UseRefCount);
            writer.Write(Caster);
            writer.Write(AddedBy);
            writer.Write(BuffID);
            writer.Write(DurationMS);
        }
    }
}