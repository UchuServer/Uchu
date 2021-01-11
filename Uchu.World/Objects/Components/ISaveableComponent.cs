using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Objects.Components
{
    public interface ISavableComponent
    {
        public Task SaveAsync(UchuContext context);
    }
}