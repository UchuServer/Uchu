using System;

namespace Uchu.World
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequireComponentAttribute : Attribute
    {
        public readonly bool Priority;
        public readonly Type Type;

        public RequireComponentAttribute(Type type, bool priority = false)
        {
            Type = type;
            Priority = priority;
        }
    }
}