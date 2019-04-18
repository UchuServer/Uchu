namespace Uchu.Core
{
    public enum PlatformState : uint
    {
        /*
         * The states here need to be investigated more.
         */

        /// <summary>
        ///     Moving, most values work as Move??
        /// </summary>
        Move = 2,

        /// <summary>
        ///     Idle
        /// </summary>
        Idle = 4,

        /// <summary>
        ///     This makes it stick to the first position?
        /// </summary>
        Down = 5,

        /// <summary>
        ///     This makes it stick to the last position?
        /// </summary>
        Up = 6,

        /// <summary>
        ///     This makes it move down but not up?
        /// </summary>
        DownNotUp = 10,

        /// <summary>
        ///     This makes it stuck?
        /// </summary>
        NotDownUp = 11,

        /// <summary>
        ///     Same as Down?
        /// </summary>
        DownTwelve = 12
    }
}