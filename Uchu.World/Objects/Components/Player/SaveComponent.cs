using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World.Objects.Components;

namespace Uchu.World
{
    /// <summary>
    /// Component used to save all savable components. <see cref="ISavableComponent"/> for more info.
    /// </summary>
    public class SaveComponent : Component
    {
        protected SaveComponent()
        {
        }
        
        /// <summary>
        /// Lock used for each time <see cref="SaveAsync"/> is called.
        /// </summary>
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Saves the game object by finding all <see cref="ISavableComponent"/> components and calling
        /// <see cref="SaveAsync"/> on them.
        /// </summary>
        public async Task SaveAsync()
        {
            await _lock.WaitAsync();
            try
            {
                await using var uchuContext = new UchuContext();
                foreach(var savableComponent in GameObject
                    .GetAllComponents().Where(c => c is ISavableComponent))
                {
                    await ((ISavableComponent) savableComponent).SaveAsync(uchuContext);
                }
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}