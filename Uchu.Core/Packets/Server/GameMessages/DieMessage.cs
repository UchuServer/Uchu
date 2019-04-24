using RakDotNet;
using Uchu.Core;

namespace Uchu.Core
{
    public class DieMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0025;

        public bool ClientDeath;

        public bool SpawnLoot = true;

        public string DeathType;

        public float DirectionAngleXZ;

        public float DirectionAngleY;

        public float DirectionForce;

        public long KillerObjectId;

        public long LootOwnerId;

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(ClientDeath);
            stream.WriteBit(SpawnLoot);
            stream.WriteBit(false);
            
            stream.WriteULong((ulong) DeathType.Length);
            stream.WriteString(DeathType, DeathType.Length, true);
            
            stream.WriteFloat(DirectionAngleXZ);
            stream.WriteFloat(DirectionAngleY);
            stream.WriteFloat(DirectionForce);
            
            stream.WriteInt32(1);

            stream.WriteLong(KillerObjectId > 1 ? KillerObjectId : ObjectId);

            if (LootOwnerId > 1)
            {
                stream.WriteBit(true);
                stream.WriteLong(LootOwnerId);
            }
            else
            {
                stream.WriteBit(false);
            }
        }
    }
}