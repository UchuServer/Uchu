using RakDotNet.IO;

namespace Uchu.World
{
    public class SetStunnedMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetStunned;
        
        public GameObject Originator { get; set; }
        
        public StunState StunState { get; set; }
        
        public bool CantAttack { get; set; }
        public bool CantAttackOutChangeWasApplied { get; set; }
        
        public bool CantEquip { get; set; }
        public bool CantEquipOutChangeWasApplied { get; set; }

        public bool CantInteract { get; set; }
        public bool CantInteractOutChangeWasApplied { get; set; }
        
        public bool CantJump { get; set; }
        public bool CantJumpOutChangeWasApplied { get; set; }
        
        public bool CantMove { get; set; }
        public bool CantMoveOutChangeWasApplied { get; set; }
        
        public bool CantTurn { get; set; }
        public bool CantTurnOutChangeWasApplied { get; set; }
        
        public bool CantUseItem { get; set; }
        public bool CantUseItemOutChangeWasApplied { get; set; }
        
        public bool DontTerminateInteract { get; set; }
        public bool IgnoreImmunity { get; set; } = true;
        
        public override void SerializeMessage(BitWriter writer)
        {
            if (writer.Flag(Originator != default))
                writer.Write(Originator);

            writer.Write((int) StunState);

            writer.WriteBit(CantAttack);
            writer.WriteBit(CantAttackOutChangeWasApplied);

            writer.WriteBit(CantEquip);
            writer.WriteBit(CantEquipOutChangeWasApplied);

            writer.WriteBit(CantInteract);
            writer.WriteBit(CantInteractOutChangeWasApplied);

            writer.WriteBit(CantJump);
            writer.WriteBit(CantJumpOutChangeWasApplied);

            writer.WriteBit(CantMove);
            writer.WriteBit(CantMoveOutChangeWasApplied);

            writer.WriteBit(CantTurn);
            writer.WriteBit(CantTurnOutChangeWasApplied);

            writer.WriteBit(CantUseItem);
            writer.WriteBit(CantUseItemOutChangeWasApplied);

            writer.WriteBit(DontTerminateInteract);
            writer.WriteBit(IgnoreImmunity);
        }
    }
}