namespace Uchu.World
{
    public readonly struct Mask
    {
        public readonly long Value;

        public Mask(long value)
        {
            Value = value;
        }

        public static Mask operator |(Mask a, Mask b)
        {
            return a.Value | b.Value;
        }

        public static Mask operator &(Mask a, Mask b)
        {
            return a.Value & b.Value;
        }

        public static Mask operator ~(Mask a)
        {
            return ~a.Value;
        }

        public static Mask operator +(Mask a, Mask b)
        {
            return a.Value | b.Value;
        }

        public static Mask operator -(Mask a, Mask b)
        {
            return a.Value & ~ b.Value;
        }

        public static bool operator ==(Mask a, Mask b)
        {
            return (a.Value & b.Value) != 0;
        }

        public static bool operator !=(Mask a, Mask b)
        {
            return (a.Value & b.Value) == 0;
        }

        public static implicit operator long(Mask mask)
        {
            return mask.Value;
        }

        public static implicit operator Mask(long value)
        {
            return new Mask(value);
        }

        public bool Equals(Mask other)
        {
            return Value == other.Value;
        }

        public bool Equals(long value)
        {
            return Value == value;
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Mask m => (m.Value == Value),
                long l => (l == Value),
                _ => false
            };
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}