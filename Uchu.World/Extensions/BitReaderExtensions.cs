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
            
            foreach (var gameObject in zone.GameObjects)
            {
                if (gameObject.ObjectId == id) return gameObject;
            }

            return null;
        }
    }
}