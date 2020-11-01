namespace Uchu.Core
{
    public abstract class HandlerGroup
    {
        /// <summary>
        ///     The server this handler group is attached to.
        /// </summary>

        private readonly object _lock = new object();
        protected Server Server { get; private set; }

        public void SetServer(Server server)
        {
            lock(_lock)
                Server = server;
        }
    }
}