using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    [ZoneSpecific(1001)]
    public class CorruptedSentry : NativeScript
    {
        /// <summary>
        /// Loads the script.
        /// </summary>
        public override Task LoadAsync()
        {
            foreach (var obj in Zone.Objects)
            {
                InitializeCorruptedSentry(obj);
            }
            Listen(Zone.OnObject,InitializeCorruptedSentry);
            
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Initializes the faction and enemies of the
        /// corrupted sentry.
        /// </summary>
        /// <param name="obj">Corrupted sentry game object to initialize.</param>
        private void InitializeCorruptedSentry(Object obj)
        {
            if (!(obj is GameObject gameObject)) return;
            if (gameObject.Lot != 8433) return;
            if (!gameObject.TryGetComponent<DestroyableComponent>(out var destroyableComponent)) return;
            destroyableComponent.Factions = new[] {4};
            destroyableComponent.Enemies = new[] {1};
        }
    }
}