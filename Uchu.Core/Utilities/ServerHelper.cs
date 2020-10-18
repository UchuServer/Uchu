using System;
using System.Threading.Tasks;
using Uchu.Api.Models;

namespace Uchu.Core
{
    public static class ServerHelper
    {
        public static async Task<InstanceInfo> RequestWorldServerAsync(UchuServer uchuServer, ZoneId zoneId)
        {
            if (uchuServer == default)
            {
                throw new ArgumentNullException(nameof(uchuServer));
            }
            
            var seekWorld = await uchuServer.Api.RunCommandAsync<SeekWorldResponse>(
                uchuServer.MasterApi, $"master/seek?z={(int) zoneId}"
            ).ConfigureAwait(false);

            if (!seekWorld.Success)
            {
                Logger.Error(seekWorld.FailedReason);

                throw new Exception(seekWorld.FailedReason);
            }

            var info = await uchuServer.Api.RunCommandAsync<InstanceInfoResponse>(
                uchuServer.MasterApi, $"instance/target?i={seekWorld.Id}"
            ).ConfigureAwait(false);

            return info.Info;
        }
    }
}