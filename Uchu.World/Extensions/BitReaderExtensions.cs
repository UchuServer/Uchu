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

            var gameObject = zone.GameObjects.FirstOrDefault(g => g.Id == id);

            return gameObject;
        }

        public static T ReadGameObject<T>(this BitReader @this, Zone zone) where T : GameObject
        {
            return @this.ReadGameObject(zone) as T;
        }

        public static void Align(this BitReader @this)
        {
            var toRead = 8 - (((@this.Position - 1) & 7) + 1);

            for (var i = 0; i < toRead; i++)
            {
                @this.ReadBit();
            }
        }
    }
}