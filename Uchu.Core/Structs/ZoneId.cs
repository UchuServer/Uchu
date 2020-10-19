using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Uchu.Core
{
    [SuppressMessage("ReSharper", "CA2225")]
    public readonly struct ZoneId : IEquatable<ZoneId>
    {
        public ushort Id { get; }
        
        public ZoneId(ushort id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            return Id.Equals(obj);
        }

        public bool Equals(ZoneId other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static implicit operator ushort(ZoneId zoneId)
        {
            return zoneId.Id;
        }
        
        public static implicit operator ZoneId(ushort id)
        {
            return new ZoneId(id);
        }

        public override string ToString()
        {
            return Id.ToString(CultureInfo.InvariantCulture);
        }

        public ushort ToUInt16()
        {
            return Id;
        }

        public static bool operator ==(ZoneId left, ZoneId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ZoneId left, ZoneId right)
        {
            return !(left == right);
        }
    }
}