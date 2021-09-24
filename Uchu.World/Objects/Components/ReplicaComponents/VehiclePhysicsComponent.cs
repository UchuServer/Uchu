using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class VehiclePhysicsComponent : StructReplicaComponent<VehiclePhysicsConstruction, VehiclePhysicsSerialization>
    {
        public override ComponentId Id => ComponentId.VehiclePhysicsComponent;

        public byte ControlScheme { get; set; } = 5;

        // this all does not work i think

        /*
        public override void Construct(BitWriter writer)
        {
            Logger.Information("Construct Vehicle Physics");
            // from VehiclePhysicsComponent.hpp in OpCrux
            writer.WriteBit(true); // flag
            writer.Write(new Vector3(0, 300, 0)); // writer.Write(this.GameObject.Transform.Position);
            writer.Write(this.GameObject.Transform.Rotation);
            writer.WriteBit(true); // ground
            writer.WriteBit(false); // rail
            writer.WriteBit(true); // lin veloc
            writer.Write(Vector3.Zero);
            writer.WriteBit(true); // angular veloc
            writer.Write(Vector3.Zero);
            writer.WriteBit(false); // local space info
            writer.WriteBit(false); // remote input info
            writer.Write(0.0f); // ping
            // // serialilze: write false

            // construct: write
            writer.Write(ControlScheme);
            writer.WriteBit(true);
            writer.WriteBit(false);

            // writer.Write(70);//NO
            // writer.WriteBit(true); // flag
            // writer.Write(this.GameObject.Transform.Position);
            // writer.Write(this.GameObject.Transform.Rotation);
            // writer.WriteBit(true); // ground
            // writer.WriteBit(false); // rail
            // writer.WriteBit(true); // lin veloc
            // writer.Write(Vector3.Zero);
            // writer.WriteBit(true); // angular veloc
            // writer.Write(Vector3.Zero);
            // writer.WriteBit(false); // local space info
            // writer.WriteBit(false); // remote input info
            // writer.Write(0.0f); // ping
            // serialilze: write false

            // writer.WriteBit(false);
            // Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            Logger.Information("Serialize Vehicle Physics");
            // writer.WriteBit(false);
            // return;
            // from VehiclePhysicsComponent.hpp in OpCrux
            writer.WriteBit(true); // flag
            writer.Write(new Vector3(0, 300, 0)); // writer.Write(this.GameObject.Transform.Position);
            writer.Write(this.GameObject.Transform.Rotation);
            writer.WriteBit(true); // ground
            writer.WriteBit(false); // rail
            writer.WriteBit(true); // lin veloc
            writer.Write(Vector3.Zero);
            writer.WriteBit(true); // angular veloc
            writer.Write(Vector3.Zero);
            writer.WriteBit(false); // local space info
            writer.WriteBit(false); // remote input info
            writer.Write(0.0f); // ping
            // serialilze: write false
            writer.WriteBit(true);

            // construct: write
            // writer.Write(ControlScheme);
            // writer.WriteBit(true);

            writer.WriteBit(false);
            // Serialize(writer);
        }
*/
        public override VehiclePhysicsConstruction GetConstructPacket()
        {
            var packet = base.GetConstructPacket();
            packet.VehicleFrameStats = new VehicleFrameStats
            {
                Position = new Vector3(300, 300, 0),
                Rotation = this.GameObject.Transform.Rotation,
                AngularVelocity = Vector3.Zero,
                LinearVelocity = Vector3.Zero,
                IsOnGround = true,
                IsOnRail = false,
                RemoteInputPing = 0.0f,
            };
            packet.EndOfRaceBehaviorType = (EndOfRaceBehaviorType) 5;
            packet.IsInputLocked = true;
            return packet;
        }

        public override VehiclePhysicsSerialization GetSerializePacket()
        {
            var packet = base.GetSerializePacket();
            Logger.Information(this.GameObject.Transform.Position);
            var teleportInfo = new VehicleFrameStatsTeleportInfo
            {
                VehicleFrameStats = new VehicleFrameStats
                {
                    Position = this.GameObject.Transform.Position, // Vector3(300, 300, 0),
                    Rotation = this.GameObject.Transform.Rotation,
                    AngularVelocity = Vector3.Zero,
                    LinearVelocity = Vector3.Zero,
                    IsOnGround = true,
                    IsOnRail = false,
                    RemoteInputPing = 0.0f,
                },
                IsTeleporting = true,
            };
            packet.VehicleFrameStatsTeleportInfo = teleportInfo;
            return packet;
        }
    }

    public struct VehiclePhysicsConstruction {
        [Default]
        public VehicleFrameStats VehicleFrameStats { get; set; }
        // write as byte !
        public EndOfRaceBehaviorType EndOfRaceBehaviorType { get; set; }
        public bool IsInputLocked { get; set; }
        public bool WheelLockExtraFriction { get; set; }
    }

    public struct VehiclePhysicsSerialization {
        [Default]
        public VehicleFrameStatsTeleportInfo VehicleFrameStatsTeleportInfo { get; set; }
        // [Default] bool with flag? should this be here
        public bool WheelLockExtraFriction { get; set; }
    }

    [Struct]
    public struct VehicleFrameStats
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public bool IsOnGround { get; set; }
        public bool IsOnRail { get; set; }
        [Default]
        public Vector3 LinearVelocity { get; set; }
        [Default]
        public Vector3 AngularVelocity { get; set; }
        [Default]
        public LocalSpaceInfo LocalSpaceInfo { get; set; }
        [Default]
        public RemoteInputInfo RemoteInputInfo { get; set; }
        public float RemoteInputPing { get; set; }
    }

    [Struct]
    public struct LocalSpaceInfo {
        public GameObject ObjectId { get; set; }
        public Vector3 Position { get; set; }
        [Default]
        public Vector3 LinearVelocity { get; set; }
    }

    [Struct]
    public struct RemoteInputInfo {
        public float RemoteInputX { get; set; }
        public float RemoteInputY { get; set; }
        public bool DoPowerslide { get; set; }
        public bool IsModified { get; set; }
    }

    [Struct]
    public struct VehicleFrameStatsTeleportInfo {
        public VehicleFrameStats VehicleFrameStats { get; set; }
        public bool IsTeleporting { get; set; }
    }

    public enum EndOfRaceBehaviorType : byte {
        DriveStraight,
        StopStraight,
        SlideLeft,
        SlideRight,
        Do360Left,
        Do360Right,
        TwoWheels,
        Jump,
    }
}
