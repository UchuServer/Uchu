using System;
using Uchu.Core;

namespace Uchu.World.Scripting.Native
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZoneSpecificAttribute : Attribute
    {
        public readonly ZoneId ZoneId;

        public ZoneSpecificAttribute(ZoneId zoneId)
        {
            ZoneId = zoneId;
        }

        public ZoneSpecificAttribute(int zoneId)
        {
            ZoneId = (ZoneId) zoneId;
        }
    }
}