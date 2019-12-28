namespace Uchu.World.Filters
{
    public interface IPerspectiveFilter
    {
        void Initialize(Player player);
        
        /// <summary>
        ///     Should this perspective view this GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject</param>
        /// <returns>Should view</returns>
        bool View(GameObject gameObject);
    }
}