using System.Collections.Generic;

namespace Uchu.World
{
    public class Zone : Object
    {
        public readonly List<Object> Objects = new List<Object>();

        public readonly List<GameObject> GameObjects = new List<GameObject>();

        public Zone()
        {
            Zone = this;
        }
    }
}