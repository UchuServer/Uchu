using System.Linq;
using RakDotNet.IO;

namespace Uchu.World
{
    public static class BitReaderExtensions
    {
        public static GameObject ReadGameObject(this BitReader @this, Zone zone)
        {
            var id = @this.Read<long>();

            if (id == -1) return null;

            var gameObject = zone.GameObjects.FirstOrDefault(g => g.ObjectId == id);

            return gameObject;
        }

        public static T ReadGameObject<T>(this BitReader @this, Zone zone) where T : GameObject
        {
            return @this.ReadGameObject(zone) as T;
        }

        public static bool Flag(this BitReader @this)
        {
            return @this.ReadBit();
        }
    }
}