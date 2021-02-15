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
        /// Instructs the component to start saving
        /// </summary>
        public void StartSaving()
        {
            _savable = true;
        }

        /// <summary>
        /// Whether the component may save
        /// </summary>
        private bool _savable;

        /// <summary>
        /// Saves the game object by finding all <see cref="ISavableComponent"/> components and calling
        /// <see cref="SaveAsync"/> on them.
        /// </summary>
        /// <param name="continueSaving">Whether to allow saves after this save</param>
        /// <remarks>
        /// Note that once continueSaving has been set to false, this behavior can not be undone and the
        /// component is rendered useless. This is to ensure that after an important event (the player leaving the server)
        /// stuff is saved no more than once to prevent corruptions.
        /// </remarks>
        public async Task SaveAsync(bool continueSaving = true)
        {
            if (!_savable)
                return;

            await _lock.WaitAsync();
            try
            {
                await using var uchuContext = new UchuContext();
                foreach(var savableComponent in GameObject
                    .GetAllComponents().Where(c => c is ISavableComponent))
                {
                    await ((ISavableComponent) savableComponent).SaveAsync(uchuContext);
                }

                // This check has to be done twice in case of race conditions
                if (_savable)
                    await uchuContext.SaveChangesAsync();
            }
            finally
            {
                if (!continueSaving)
                    _savable = false;
                
                _lock.Release();
            }
        }
    }
}