using System;
using System.Linq;

namespace Uchu.Core
{
    public static class Utils
    {
        private static readonly Random Random = new Random();

        public static long RandomLong(long min, long max)
        {
            var buf = new byte[8];

            Random.NextBytes(buf);

            var res = BitConverter.ToInt64(buf, 0);

            return Math.Abs(res % (max - min)) + min;
        }

        public static long GenerateObjectId()
            => RandomLong(1000000000000000000, 1999999999999999999);

        public static long GenerateSpawnerId()
            => RandomLong(70000000000000, 79999999999999);

        public static ZoneChecksum GetChecksum(ZoneId id)
        {
            var name = Enum.GetName(typeof(ZoneId), id);

            var names = Enum.GetNames(typeof(ZoneChecksum));
            var values = Enum.GetValues(typeof(ZoneChecksum));

            return (ZoneChecksum) values.GetValue(names.ToList().IndexOf(name));
        }
    }
}