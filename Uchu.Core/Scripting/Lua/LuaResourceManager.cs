using System;
using System.Numerics;

namespace Uchu.Core.Scripting.Lua
{
    // RESMGR
    public class LuaResourceManager
    {
        private WorldInstance _instance;

        public LuaResourceManager(WorldInstance instance)
        {
            _instance = instance;
        }

        // RESMGR:LoadObject { objectTemplate = int, x = int, y = int, z = int, rw = int, rx = int, ry = int, rz = int } : void
        public void LoadObject(dynamic obj)
        {
            int lot = obj.objectTemplate;
            var position = new Vector3
            {
                X = obj.x,
                Y = obj.y,
                Z = obj.z
            };
            var rotation = new Vector4
            {
                X = obj.rx,
                Y = obj.ry,
                Z = obj.rz,
                W = obj.rw
            };

            Console.WriteLine($"RESMGR:LoadObject {{ objectTemplate = {lot}, (x, y, z) = {position}, (rx, ry, rz, rw) = {rotation} }}");
        }
    }
}