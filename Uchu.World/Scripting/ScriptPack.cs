using System.Threading.Tasks;

namespace Uchu.World.Scripting
{
    public abstract class ScriptPack
    {
        public Zone Zone { get; }
        
        public string Location { get; }
        
        public abstract string Name { get; }

        public ScriptPack(Zone zone, string location)
        {
            Zone = zone;

            Location = location;
        }
        
        public abstract Task LoadAsync();

        public abstract Task UnloadAsync();

        public abstract Task ReloadAsync();
    }
}