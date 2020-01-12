using System;
using Uchu.Core;

namespace Uchu.World.Scripting.Native
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