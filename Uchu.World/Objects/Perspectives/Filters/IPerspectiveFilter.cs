namespace Uchu.World.Filters
{
    public interface IPerspectiveFilter
    {
        Player Player { get; set; }

        void Start(){}
        
        /// <summary>
        ///     Should this perspective view this GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject</param>
        /// <returns>Should view</returns>
        bool View(GameObject gameObject);
    }
}