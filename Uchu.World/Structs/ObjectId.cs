using System;

namespace Uchu.World
{
    public struct ObjectId
    {
        private static readonly Random Random = new Random();

        public ulong Id { get; set; }

        public ObjectId(ulong id)
        {
            Id = id;
        }
        
        public static implicit operator ulong(ObjectId objectId)
        {
            return objectId.Id;
        }

        public static implicit operator ObjectId(ulong id)
        {
            return new ObjectId(id);
        }
        
        public static implicit operator long(ObjectId objectId)
        {
            return (long) objectId.Id;
        }
        
        public static implicit operator ObjectId(long id)
        {
            return new ObjectId((ulong) id);
        }

        public static implicit operator (uint id, uint flags)(ObjectId objectId)
        {
            var id = (uint) (objectId & uint.MaxValue);
            var flags = (uint) (objectId >> 32);

            return (id, flags);
        }

        public static ObjectId NewObjectId(ObjectIdFlags flags)
        {
            var id = (long) Random.Next(1000000000, 2000000000);
            
            id |= (long) flags;

            return id;
        }
    }
}