using System.Threading.Tasks;

namespace Uchu.World.Scripting
{
    public abstract class Script : ObjectBase
    {
        public abstract Task LoadAsync();

        public abstract Task UnloadAsync();
    }
}