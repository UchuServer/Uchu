using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Objects.Components
{
    /// <summary>
    /// Allows one to save a component to the Uchu database
    /// </summary>
    public interface ISavableComponent
    {
        /// <summary>
        /// Saves this component to the Uchu database
        /// </summary>
        /// <param name="context">The database context to save to</param>
        public Task SaveAsync(UchuContext context);
    }
}