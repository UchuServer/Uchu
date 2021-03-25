using System;
using Uchu.Api;
using Uchu.Api.Models;

namespace Uchu.Core.Api
{
    public class InstanceCommands
    {
        public UchuServer UchuServer { get; }
        
        public InstanceCommands(UchuServer uchuServer)
        {
            UchuServer = uchuServer;
        }

        [ApiCommand("server/verify")]
        public object ReadySetupCallback()
        {
            var response = new ReadyCallbackResponse();

            if (UchuServer.Id == Guid.Empty)
            {
                response.FailedReason = "invalid id";

                return response;
            }

            if (!UchuServer.RakNetServer.TcpStarted)
            {
                response.FailedReason = "not ready";

                return response;
            }

            response.Success = true;
            
            return response;
        }
    }
}