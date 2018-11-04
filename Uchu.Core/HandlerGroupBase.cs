namespace Uchu.Core
{
    public abstract class HandlerGroupBase
    {
        protected Server Server { get; private set; }

        internal void SetServer(Server server)
        {
            Server = server;
        }
    }
}