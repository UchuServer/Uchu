using System.Linq;

namespace Uchu.Core.Scripting.Lua
{
    // GAMEOBJ
    public class LuaObjectManager
    {
        private readonly WorldInstance _instance;

        public LuaTimerManager TimerManager { get; }

        public LuaObjectManager(WorldInstance instance)
        {
            _instance = instance;

            TimerManager = new LuaTimerManager();
        }

        // GAMEOBJ:GetTimer() : LuaTimers
        public LuaTimerManager GetTimer()
            => TimerManager;

        // GAMEOBJ:GetObjectByID(string) : object
        public LuaGameObject GetObjectByID(string objectId)
            => _instance.Objects.First(obj => obj.ObjectId == long.Parse(objectId)).LuaObject;

        // GAMEOBJ:GenerateSpawnedID() : string
        public string GenerateSpawnedID()
            => Utils.GenerateObjectId().ToString();
    }
}