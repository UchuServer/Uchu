using System;

namespace Uchu.World
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequireComponentAttribute : Attribute
    {
        public bool Priority { get; }
        public Type Type { get; }

        public RequireComponentAttribute(Type type, bool priority = false)
        {
            Type = type;
            Priority = priority;
        }
    }
}