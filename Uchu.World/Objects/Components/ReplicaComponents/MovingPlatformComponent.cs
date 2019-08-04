using System;
using System.Linq;
using System.Numerics;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    public class MovingPlatformComponent : ReplicaComponent
    {
        public MovingPlatformPath Path { get; set; }
        
        public string PathName { get; set; }
        
        public uint PathStart { get; set; }
        
        public PlatformType Type { get; set; }

        public PlatformState State { get; set; } = PlatformState.Idle;
        
        public Vector3 TargetPosition { get; set; }
        
        public Quaternion TargetRotation { get; set; }
        
        public uint CurrentWaypointIndex { get; set; }
        
        public uint NextWaypointIndex { get; set; }
        
        public float IdleTimeElapsed { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.MovingPlatform;

        public override void FromLevelObject(LevelObject levelObject)
        {
            PathName = levelObject.Settings.TryGetValue("attached_path", out var name) ? (string) name : "";
            PathStart = levelObject.Settings.TryGetValue("attached_path_start", out var start)
                ? (uint) start
                : 0;

            Path = Zone.ZoneInfo.Paths.FirstOrDefault(p => p is MovingPlatformPath && p.Name == PathName) as
                MovingPlatformPath;
            
            Type = levelObject.Settings.TryGetValue("platformIsMover", out var isMover) && (bool) isMover
                ? PlatformType.Mover
                : levelObject.Settings.TryGetValue("platformIsSimpleMover", out var isSimpleMover) &&
                  (bool) isSimpleMover
                    ? PlatformType.SimpleMover
                    : PlatformType.None;

            CurrentWaypointIndex = PathStart;

            State = PlatformState.Idle;
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);

            var hasPath = PathName != null;

            writer.WriteBit(hasPath);

            if (hasPath)
            {
                writer.WriteBit(true);
                writer.Write((ushort) PathName.Length);
                writer.WriteString(PathName, PathName.Length, true);
                writer.Write(PathStart);
                writer.WriteBit(false);
            }

            var hasPlatform = Type != PlatformType.None;
            
            writer.WriteBit(hasPlatform);
            
            if (!hasPlatform) return;

            writer.Write((uint) Type);

            switch (Type)
            {
                case PlatformType.None:
                    break;
                
                case PlatformType.Mover:
                    writer.WriteBit(true);

                    writer.Write((uint) State);

                    writer.Write(-1);
                    writer.WriteBit(false);
                    writer.WriteBit(CurrentWaypointIndex != 0);
                    writer.Write<float>(0);

                    writer.Write(TargetPosition);

                    writer.Write(CurrentWaypointIndex);
                    writer.Write(NextWaypointIndex);

                    writer.Write(IdleTimeElapsed);
                    writer.Write<uint>(0);
                    break;
                
                case PlatformType.SimpleMover:
                    writer.WriteBit(true);
                    writer.WriteBit(true);

                    writer.Write(TargetPosition);
                    writer.Write(TargetRotation);

                    writer.WriteBit(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}