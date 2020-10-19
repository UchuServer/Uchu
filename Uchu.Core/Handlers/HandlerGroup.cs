namespace Uchu.Core
{
    public abstract class HandlerGroup
    {
        /// <summary>
        ///     The server this handler group is attached to.
        /// </summary>

        private readonly object _lock = new object();
        protected UchuServer UchuServer { get; private set; }

        public void SetServer(UchuServer uchuServer)
        {
            lock(_lock)
                UchuServer = uchuServer;
        }
    }
}