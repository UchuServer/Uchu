using System;

namespace Uchu.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AutoSerializeAttribute : Attribute
    {
        public bool Compressed { get; set; } = false;
        public Type Type { get; set; } = null;
        public int Length { get; set; } = 33;
        public bool Unsigned { get; set; } = false;
        public bool Wide { get; set; } = false;
        public bool Bool { get; set; } = false;

        public AutoSerializeAttribute()
        {
        }

        public AutoSerializeAttribute(Type type)
        {
            Type = type;
        }
    }
}