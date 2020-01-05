using System.Threading.Tasks;

namespace Uchu.World.Filters
{
    public class MaskFilter : IPerspectiveFilter
    {
        public Mask ViewMask { get; set; }

        public void Initialize(Player player)
        {
        }

        public Task Tick()
        {
            return Task.CompletedTask;
        }

        public bool View(GameObject gameObject)
        {
            return gameObject.Layer == ViewMask;
        }
    }
}