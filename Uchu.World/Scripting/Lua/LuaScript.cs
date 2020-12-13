using System.Threading.Tasks;

namespace Uchu.World.Scripting.Lua
{
    public abstract class LuaScript : ObjectBase
    {
        public abstract Task LoadAsync(GameObject self);

        public abstract Task UnloadAsync();
    }
}