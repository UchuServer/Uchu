using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class MovingPlatformComponent : ReplicaComponent
    {
        /// <summary>
        ///     Current timer.
        /// </summary>
        private Timer _timer;

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

        public override ComponentId Id => ComponentId.MovingPlatformComponent;

        /// <summary>
        ///     Next Path Index
        /// </summary>
        private uint NextIndex => CurrentWaypointIndex + 1 > Path.Waypoints.Length - 1 ? 0 : CurrentWaypointIndex + 1;

        /// <summary>
        ///     Current WayPoint
        /// </summary>
        public MovingPlatformWaypoint WayPoint => Path.Waypoints[CurrentWaypointIndex] as MovingPlatformWaypoint;

        /// <summary>
        ///     Next WayPoint
        /// </summary>
        public MovingPlatformWaypoint NextWayPoint => Path.Waypoints[NextIndex] as MovingPlatformWaypoint;

        protected MovingPlatformComponent()
        {
            OnStart.AddListener(() =>
            {
                PathName = GameObject.Settings.TryGetValue("attached_path", out var name) ? (string) name : "";
                PathStart = GameObject.Settings.TryGetValue("attached_path_start", out var start)
                    ? (uint) start
                    : 0;

                Path = Zone.ZoneInfo.Paths.FirstOrDefault(p => p is MovingPlatformPath && p.Name == PathName) as
                    MovingPlatformPath;

                Type = GameObject.Settings.TryGetValue("platformIsMover", out var isMover) && (bool) isMover
                    ? PlatformType.Mover
                    : GameObject.Settings.TryGetValue("platformIsSimpleMover", out var isSimpleMover) &&
                      (bool) isSimpleMover
                        ? PlatformType.SimpleMover
                        : PlatformType.None;

                CurrentWaypointIndex = PathStart;

                State = PlatformState.Idle;
                
                Task.Run(WaitPoint);
            });
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

        private void MovePlatform()
        {
            /*
             * Update Object in world.
             */
            PathName = Path.Name;
            State = PlatformState.Move;
            TargetPosition = WayPoint.Position;
            TargetRotation = WayPoint.Rotation;
            NextWaypointIndex = NextIndex;

            Update(GameObject);

            /*
             * Start Waiting after completing path.
             */
            _timer = new Timer
            {
                AutoReset = false,
                Interval = WayPoint.Speed * 1000
            };

            _timer.Elapsed += (sender, args) => { WaitPoint(); };

            Task.Run(() => _timer.Start());
        }

        private void WaitPoint()
        {
            // Move to next path index.
            CurrentWaypointIndex = NextIndex;

            /*
             * Update Object in world.
             */
            PathName = null;
            State = PlatformState.Idle;
            TargetPosition = WayPoint.Position;
            TargetRotation = WayPoint.Rotation;
            NextWaypointIndex = NextIndex;

            Update(GameObject);

            /*
             * Start Waiting after waiting.
             */
            _timer = new Timer
            {
                AutoReset = false,
                Interval = WayPoint.WaitTime * 1000
            };

            _timer.Elapsed += (sender, args) => { MovePlatform(); };

            Task.Run(() => _timer.Start());
        }
    }
}