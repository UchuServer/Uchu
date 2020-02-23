using Uchu.Api;
using Uchu.Api.Models;

namespace Uchu.Core.Api
{
    public class InstanceCommands
    {
        public Server Server { get; }
        
        public InstanceCommands(Server server)
        {
            Server = server;
        }

        [ApiCommand("server/verify")]
        public object ReadySetupCallback()
        {
            return new ReadyCallbackResponse
            {
                Success = true
            };
        }
    }
}