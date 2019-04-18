using System;
using System.Numerics;
using RakDotNet;

namespace Uchu.Core
{
    public class MovingPlatformComponent : ReplicaComponent
    {
        public MovingPlatformPath Path { get; set; }

        public string PathName { get; set; } = null;
        public uint PathStart { get; set; } = 0;
        public PlatformType Type { get; set; } = PlatformType.None;

        public PlatformState State { get; set; } = PlatformState.Idle;
        public Vector3 TargetPosition { get; set; }
        public Vector4 TargetRotation { get; set; }
        public uint CurrentWaypointIndex { get; set; }
        public uint NextWaypointIndex { get; set; }
        public float IdleTimeElapsed { get; set; } = 0;

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(true);

            var hasPath = PathName != null;

            stream.WriteBit(hasPath);

            if (hasPath)
            {
                stream.WriteBit(true);
                stream.WriteUShort((ushort) PathName.Length);
                stream.WriteString(PathName, PathName.Length, true);
                stream.WriteUInt(PathStart); // guess
                stream.WriteBit(false);
            }

            var hasPlatform = Type != PlatformType.None;

            stream.WriteBit(hasPlatform);

            if (!hasPlatform) return;
            stream.WriteUInt((uint) Type);

            switch (Type)
            {
                case PlatformType.Mover:
                    stream.WriteBit(true);

                    stream.WriteUInt((uint) State);

                    stream.WriteInt(-1);
                    stream.WriteBit(false);
                    stream.WriteBit(CurrentWaypointIndex != 0); // is this how to do it?
                    stream.WriteFloat(0);

                    stream.WriteFloat(TargetPosition.X);
                    stream.WriteFloat(TargetPosition.Y);
                    stream.WriteFloat(TargetPosition.Z);

                    stream.WriteUInt(CurrentWaypointIndex);
                    stream.WriteUInt(NextWaypointIndex);

                    stream.WriteFloat(IdleTimeElapsed);
                    stream.WriteUInt(0);
                    break;

                case PlatformType.SimpleMover:
                    stream.WriteBit(true);
                    stream.WriteBit(true);

                    stream.WriteFloat(TargetPosition.X);
                    stream.WriteFloat(TargetPosition.Y);
                    stream.WriteFloat(TargetPosition.Z);

                    stream.WriteFloat(TargetRotation.X);
                    stream.WriteFloat(TargetRotation.Y);
                    stream.WriteFloat(TargetRotation.Z);
                    stream.WriteFloat(TargetRotation.W);

                    stream.WriteBit(false);
                    break;
                case PlatformType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}