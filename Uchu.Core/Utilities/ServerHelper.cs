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

        /// <summary>
        /// Allocates a new server with the new zone id,
        /// such as for a minigame.
        /// </summary>
        /// <param name="uchuServer">Base server to use for master server communication.</param>
        /// <param name="zoneId">Zone id to allocate a new server of.</param>
        /// <returns>The information about the new server.</returns>
        /// <exception cref="ArgumentNullException">Argument is missing or null.</exception>
        public static async Task<InstanceInfo> RequestNewWorldServerAsync(UchuServer uchuServer, ZoneId zoneId)
        {
            if (uchuServer == default)
            {
                throw new ArgumentNullException(nameof(uchuServer));
            }
            
            // Allocate the new zone and get the instance information.
            var allocatedServer = await uchuServer.Api.RunCommandAsync<SeekWorldResponse>(
                uchuServer.MasterApi, $"master/allocate?z={zoneId}"
            ).ConfigureAwait(false);
            var allocatedInstance = await uchuServer.Api.RunCommandAsync<InstanceInfoResponse>(
                uchuServer.MasterApi, $"instance/target?i={allocatedServer.Id}"
            ).ConfigureAwait(false);
            
            // Return the zone information.
            return allocatedInstance.Info;
        }
    }
}