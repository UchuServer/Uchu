using System;
using RakDotNet;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public class ReplicaPacket : IReplica
    {
        private string _name;
        
        public long ObjectId { get; set; }

        public int LOT { get; set; }

        /// <summary>
        /// Name of the Replica, without newline.
        /// </summary>
        public string Name
        {
            get => _name;
            // TODO: Look into this.
            set => _name = value.Replace("/r", " ").Replace("\n", " ");
        }

        public uint Created { get; set; }

        public bool HasTriggerId { get; set; } = false;

        public long SpawnerObjectId { get; set; } = -1;

        public uint SpawnerNodeId { get; set; } = 0;

        public float Scale { get; set; } = -1;

        public long ParentObjectId { get; set; } = -1;

        public long[] ChildObjectIds { get; set; } = null;

        public IReplicaComponent[] Components { get; set; }

        public LegoDataDictionary Settings { get; set; }

        private void _write(BitStream stream)
        {
            stream.WriteBit(true);

            var hasParent = ParentObjectId != -1;

            stream.WriteBit(hasParent);

            if (hasParent)
            {
                stream.WriteLong(ParentObjectId);
                stream.WriteBit(false);
            }

            var hasChildren = ChildObjectIds != null && ChildObjectIds.Length > 0;

            stream.WriteBit(hasChildren);

            if (hasChildren)
            {
                stream.WriteUShort((ushort) ChildObjectIds.Length);

                foreach (var id in ChildObjectIds) stream.WriteLong(id);
            }
        }

        public void Serialize(BitStream stream)
        {
            _write(stream);

            foreach (var component in Components) stream.WriteSerializable(component);

            if (HasTriggerId)
                stream.WriteSerializable(new TriggerComponent());
        }

        public void Construct(BitStream stream)
        {
            stream.WriteLong(ObjectId);

            stream.WriteInt(LOT);

            stream.WriteByte((byte) Name.Length);
            stream.WriteString(Name, Name.Length, true);

            stream.WriteUInt(Created);

            // TODO: figure this out
            stream.WriteBit(false); // has compressed ldf

            stream.WriteBit(HasTriggerId);

            var hasSpawner = SpawnerObjectId != -1;

            stream.WriteBit(hasSpawner);

            if (hasSpawner)
                stream.WriteLong(SpawnerObjectId);

            var hasSpawnerNode = SpawnerNodeId != 0;

            stream.WriteBit(hasSpawnerNode);

            if (hasSpawnerNode)
                stream.WriteUInt(SpawnerNodeId);

            var hasScale = !Scale.Equals(-1);

            stream.WriteBit(hasScale);

            if (hasScale)
                stream.WriteFloat(Scale);

            stream.WriteBit(false); // has object world state

            stream.WriteBit(false); // has gm level

            _write(stream);

            foreach (var component in Components) component.Construct(stream);

            if (HasTriggerId)
                new TriggerComponent().Construct(stream);
        }

        public void Deserialize(BitStream stream)
        {
            throw new NotSupportedException();
        }

        public void Destruct()
        {
            throw new NotSupportedException();
        }
    }
}