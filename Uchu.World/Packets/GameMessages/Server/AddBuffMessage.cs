using System;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct AddBuffMessage
    {
        public GameObject Associate { get; set; }
        
        public GameMessageId GameMessageId => GameMessageId.AddBuff;

        public bool AddedByTeammate { get; set; }

        public bool ApplyOnTeammates { get; set; }

        public bool CancelOnDamageAbsorbRanOut { get; set; }

        public bool CancelOnDamaged { get; set; }

        [Default(true)]
        public bool CancelOnDeath { get; set; } //default true in docs

        public bool CancelOnLogout { get; set; }

        public bool CancelOnMove { get; set; }

        [Default(true)]
        public bool CancelOnRemoveBuff { get; set; } //default true in docs, also ????

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
    }
}