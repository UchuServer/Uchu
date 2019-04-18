using System;
using System.Threading.Tasks;
using System.Timers;
using Uchu.Core;
using Uchu.Core.Scriptable;

namespace Uchu.World.Scriptable
{
    /// <inheritdoc />
    /// <summary>
    ///     Script for every moving platform
    /// </summary>
    [AutoAssign(typeof(MovingPlatformComponent))]
    public class MovingPlatformScript : GameScript
    {
        /*
         * Moving platforms have multiple states, most of which are still unknown/undocumented.
         */
        
        /*
         * public static for runtime testing.
         */
        public static PlatformState StateOn = PlatformState.Move;
        public static PlatformState StateOff = PlatformState.Idle;

        /// <summary>
        ///     MovingPlatformComponent on this Replica Object.
        /// </summary>
        private MovingPlatformComponent _component;

        /// <summary>
        ///     Current Path Index.
        /// </summary>
        private uint _index;

        /// <summary>
        ///     The MovingPlatformPath of this Replica Object's MovingPlatformComponent.
        /// </summary>
        private MovingPlatformPath _path;

        /// <summary>
        ///     SimplePhysicsComponent on this Replica Object.
        /// </summary>
        private SimplePhysicsComponent _physics;

        /// <summary>
        ///     Current timer.
        /// </summary>
        private Timer _timer;

        /// <summary>
        ///     Inherited Constructor
        /// </summary>
        /// <param name="world"></param>
        /// <param name="replicaPacket"></param>
        public MovingPlatformScript(Core.World world, ReplicaPacket replicaPacket) : base(world, replicaPacket)
        {
        }

        /// <summary>
        ///     Next Path Index
        /// </summary>
        private uint _nextIndex => _index + 1 > _path.Waypoints.Length - 1 ? 0 : _index + 1;

        /// <summary>
        ///     Current WayPoint
        /// </summary>
        public MovingPlatformWaypoint WayPoint => _path.Waypoints[_index] as MovingPlatformWaypoint;

        /// <summary>
        ///     Next WayPoint
        /// </summary>
        public MovingPlatformWaypoint NextWayPoint => _path.Waypoints[_nextIndex] as MovingPlatformWaypoint;

        public override void Start()
        {
            _component = GetComponent<MovingPlatformComponent>();
            if (_component == null)
                throw new ArgumentException();
            _path = _component.Path;

            _physics = GetComponent<SimplePhysicsComponent>();
            if (_physics != null)
                _physics.HasPosition = false;

            _index = _component.PathStart;

            /*
             * Update Object in world.
             */
            _component.Type = PlatformType.Mover;
            _component.PathName = null;
            _component.State = StateOff;
            _component.TargetPosition = WayPoint.Position;
            _component.TargetRotation = WayPoint.Rotation;
            _component.CurrentWaypointIndex = _index;
            _component.NextWaypointIndex = _nextIndex;

            World.UpdateObject(this);

            /*
             * Start Platform after waiting
             */
            _timer = new Timer
            {
                AutoReset = false,
                Interval = WayPoint.WaitTime * 1000
            };

            _timer.Elapsed += (sender, args) => { MovePlatform(); };

            Task.Run(() => _timer.Start());
        }

        private void MovePlatform()
        {
            /*
             * Update Object in world.
             */
            _component.PathName = _path.Name;
            _component.State = StateOn;
            _component.TargetPosition = WayPoint.Position;
            _component.TargetRotation = WayPoint.Rotation;
            _component.CurrentWaypointIndex = _index;
            _component.NextWaypointIndex = _nextIndex;

            World.UpdateObject(this);

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
            _index = _nextIndex;

            /*
             * Update Object in world.
             */
            _component.PathName = null;
            _component.State = StateOff;
            _component.TargetPosition = WayPoint.Position;
            _component.TargetRotation = WayPoint.Rotation;
            _component.CurrentWaypointIndex = _index;
            _component.NextWaypointIndex = _nextIndex;

            World.UpdateObject(this);

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