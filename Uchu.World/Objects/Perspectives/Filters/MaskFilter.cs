namespace Uchu.World.Filters
{
    public class MaskFilter : IPerspectiveFilter
    {
        public Mask ViewMask { get; set; }

        public Player Player { get; set; }

        public bool View(GameObject gameObject)
        {
            return gameObject.Layer == ViewMask;
        }
    }
}