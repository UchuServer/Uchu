using RakDotNet;

namespace Uchu.Core
{
    public class MovingPlatformComponent : ReplicaComponent
    {
        public string PathName { get; set; } = null;

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
                stream.WriteUInt(0);
                stream.WriteBit(false);
            }

            stream.WriteBit(false); // TODO: implement this
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}