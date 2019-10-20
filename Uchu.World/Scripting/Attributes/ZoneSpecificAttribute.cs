using System;
using Uchu.Core;

namespace Uchu.World.Scripting
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ZoneSpecificAttribute : Attribute
    {
        public readonly ZoneId ZoneId;

        public ZoneSpecificAttribute(ZoneId zoneId)
        {
            ZoneId = zoneId;
        }
    }
}