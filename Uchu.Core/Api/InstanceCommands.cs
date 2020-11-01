using System;
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
            var response = new ReadyCallbackResponse();

            if (Server.Id == Guid.Empty)
            {
                response.FailedReason = "invalid id";

                return response;
            }

            response.Success = true;
            
            return response;
        }
    }
}