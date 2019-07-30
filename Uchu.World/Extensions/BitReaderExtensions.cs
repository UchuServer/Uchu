using System.Linq;
using RakDotNet.IO;

namespace Uchu.World
{
    public static class BitReaderExtensions
    {
        public static GameObject ReadGameObject(this BitReader @this, Zone zone)
        {
            return zone.GameObjects.FirstOrDefault(g => g.ObjectId == @this.Read<long>());
        }
    }
}