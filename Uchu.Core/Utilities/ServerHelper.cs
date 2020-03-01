using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Api.Models;

namespace Uchu.Core
{
    public static class ServerHelper
    {
        public static async Task<InstanceInfo> RequestWorldServerAsync(Server server, ZoneId zoneId)
        {
            if (server == default)
            {
                throw new ArgumentNullException(nameof(server));
            }
            
            var seekWorld = await server.Api.RunCommandAsync<SeekWorldResponse>(
                server.MasterApi, $"master/seek?z={(int) zoneId}"
            ).ConfigureAwait(false);

            if (!seekWorld.Success)
            {
                Logger.Error(seekWorld.FailedReason);

                throw new Exception(seekWorld.FailedReason);
            }

            var info = await server.Api.RunCommandAsync<InstanceInfoResponse>(
                server.MasterApi, $"instance/target?i={seekWorld.Id}"
            ).ConfigureAwait(false);

            return info.Info;
        }
    }
}