using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Core;

namespace Uchu.Master.Api
{
    public class MasterCommands
    {
        [ApiCommand("subsidiary")]
        public async Task<object> Subsidiary()
        {
            var port = await MasterServer.ClaimApiPortAsync();

            MasterServer.Subsidiaries.Add(port);
            
            return new ClaimPortResponse
            {
                Success = true,
                Port = port
            };
        }

        [ApiCommand("instance/basic")]
        public async Task<object> GetBasicInstance(string typeString)
        {
            var response = new InstanceInfoResponse();
            
            if (!int.TryParse(typeString, out var type))
            {
                response.FailedReason = "invalid type";

                return response;
            }
            
            var instances = await MasterServer.GetAllInstancesAsync();

            var instance = instances.FirstOrDefault(i => i.Type == type);

            if (instance == default)
            {
                response.FailedReason = "not found";

                return response;
            }

            response.Success = true;
            response.Hosting = instance.MasterApi == MasterServer.ApiPort;
            response.Info = instance;

            return response;
        }

        [ApiCommand("instance/target")]
        public async Task<object> GetTargetInstance(string id)
        {
            var response = new InstanceInfoResponse();

            if (!Guid.TryParse(id, out var guid))
            {
                response.FailedReason = "invalid guid";
                
                return response;
            }
            
            var instances = await MasterServer.GetAllInstancesAsync();
            var instance = instances.FirstOrDefault(i => i.Id == guid);

            if (instance == default)
            {
                response.FailedReason = "not found";
                return response;
            }

            response.Success = true;
            response.Hosting = instance.MasterApi == MasterServer.ApiPort;
            response.Info = instance;
            
            return response;
        }

        [ApiCommand("instance/heartbeat")]
        public async Task<object> Heartbeat(string id)
        {
            var response = new BaseResponse();
            
            Logger.Information("Received heart beat");
            
            if ((await MasterServer.GetAllInstancesAsync()).FirstOrDefault() is {} instance
                && MasterServer.InstanceHeartBeats.ContainsKey(id))
            {
                MasterServer.InstanceHeartBeats[id]++;
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.FailedReason = "Instance does not exist.";
            }

            return response;
        }

        [ApiCommand("claim/world")]
        public async Task<object> ClaimWorldPort()
        {
            var response = new ClaimPortResponse();

            if (MasterServer.IsSubsidiary)
            {
                response.FailedReason = "is subsidiary";

                return response;
            }

            response.Success = true;
            response.Port = await MasterServer.ClaimWorldPortAsync();

            return response;
        }

        [ApiCommand("claim/api")]
        public async Task<object> ClaimApiPort()
        {
            var response = new ClaimPortResponse();

            if (MasterServer.IsSubsidiary)
            {
                response.FailedReason = "is subsidiary";

                return response;
            }

            response.Success = true;
            response.Port = await MasterServer.ClaimApiPortAsync();

            return response;
        }

        [ApiCommand("instance/list")]
        public object InstanceList()
        {
            var response = new InstanceListResponse
            {
                Success = true
            };
            
            var instances = MasterServer.Instances.Select(i => new InstanceInfo
            {
                MasterApi = MasterServer.ApiPort,
                ApiPort = i.ApiPort,
                Port = i.ServerPort,
                Id = i.Id,
                Type = (int) i.ServerType,
                Zones = i.Zones.Select(z => (int) z).ToList()
            }).ToList();
            
            response.Instances = instances;

            return response;
        }

        [ApiCommand("instance/list/complete")]
        public async Task<object> InstanceCompleteList()
        {
            var response = new InstanceListResponse
            {
                Success = true,
                Instances = await MasterServer.GetAllInstancesAsync()
            };

            return response;
        }

        [ApiCommand("instance/commission")]
        public async Task<object> CommissionInstance(string zoneId)
        {
            var response = new CommissionInstanceResponse();

            if (!int.TryParse(zoneId, out var zone))
            {
                response.FailedReason = "invalid type";
                return response;
            }

            Guid id;
            
            try
            {
                var port = await MasterServer.ClaimWorldPortAsync();
                id = await MasterServer.StartInstanceAsync(ServerType.World, port);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                response.FailedReason = "error";
                return response;
            }

            var instance = MasterServer.Instances.First(i => i.Id == id);
            instance.Zones.Add((ZoneId) zone);
            response.Success = true;
            response.Id = id;
            response.ApiPort = instance.ApiPort;

            var timeout = MasterServer.Config.ServerBehaviour.InstanceCommissionTimeout;
            var delay = 500; // time (ms) in between server/verify API calls

            while (true)
            {
                var verify = await MasterServer.Api.RunCommandAsync<ReadyCallbackResponse>(
                    instance.ApiPort, "server/verify"
                ).ConfigureAwait(false);

                if (!(verify is {Success: true}))
                {
                    timeout -= delay;

                    if (timeout <= 0)
                    {
                        throw new TimeoutException("Commission timed out");
                    }

                    await Task.Delay(delay);
                    continue;
                }

                instance.Ready = true;
                break;
            }

            return response;
        }

        [ApiCommand("master/die")]
        public void KillMaster(string error = "Fatal error; killing server.")
        {
            Logger.Error(error);

            foreach (ServerInstance instance in MasterServer.Instances.ToList())
            {
                DecommissionInstance(instance.Id.ToString());
            }
            
            if (MasterServer.Config.ServerBehaviour.PressKeyToExit)
            {
                Logger.Error("Press any key to exit...");
                Console.ReadKey();
            }
            
            Environment.Exit(1);
        }

        [ApiCommand("instance/decommission")]
        public object DecommissionInstance(string id)
        {
            var response = new BaseResponse();
            
            if (!Guid.TryParse(id, out var guid))
            {
                response.FailedReason = "invalid guid";
                
                return response;
            }

            var instances = MasterServer.Instances;

            var instance = instances.FirstOrDefault(i => i.Id == guid);

            if (instance == default)
            {
                response.FailedReason = "not found";

                return response;
            }
            
            instance.Process.Kill();

            instances.Remove(instance);

            return response;
        }

        [ApiCommand("master/seek")]
        public async Task<object> SeekWorld(string zone)
        {
            var response = new SeekWorldResponse();
            
            if (!int.TryParse(zone, out var zoneId))
            {
                response.FailedReason = "invalid zone";
                return response;
            }

            var instances = await MasterServer.GetAllInstancesAsync();

            foreach (var instance in instances.Where(instance => instance.Zones.Contains(zoneId)))
            {
                var playerInfo = await MasterServer.Api.RunCommandAsync<ZonePlayersResponse>(
                    instance.ApiPort, $"world/players?z={zoneId}"
                );
                
                // The world server crashed, decommission it
                if (playerInfo == null)
                {
                    DecommissionInstance(instance.Id.ToString());
                    continue;
                }

                if (!playerInfo.Success)
                {
                    Logger.Error(playerInfo.FailedReason);
                    throw new Exception(playerInfo.FailedReason);
                }

                if (playerInfo.Characters.Count >= playerInfo.MaxPlayers)
                    continue;
                
                response.Success = true;
                response.ApiPort = instance.ApiPort;
                response.Id = instance.Id;

                return response;
            }

            response = await MasterServer.Api.RunCommandAsync<SeekWorldResponse>(
                MasterServer.MasterPort, $"master/allocate?z={zoneId}"
            ).ConfigureAwait(false);

            return response;
        }

        [ApiCommand("master/allocate")]
        public async Task<object> AllocateWorld(string zone)
        {
            var response = new SeekWorldResponse();
            
            if (MasterServer.IsSubsidiary)
            {
                response.FailedReason = "is subsidiary";
                
                return response;
            }
            
            if (!int.TryParse(zone, out var zoneId))
            {
                response.FailedReason = "invalid zone";

                return response;
            }

            var toCheck = new List<int> {MasterServer.ApiPort};

            toCheck.AddRange(MasterServer.Subsidiaries);
            
            foreach (var subsidiary in toCheck)
            {
                var status = await MasterServer.Api.RunCommandAsync<MasterStatusResponse>(
                    subsidiary, "master/status"
                ).ConfigureAwait(false);

                if (!status.CanHostAdditionWorldInstances) continue;
                
                var details = await MasterServer.Api.RunCommandAsync<CommissionInstanceResponse>(
                    subsidiary, $"instance/commission?z={zoneId}"
                ).ConfigureAwait(false);

                response.Success = true;
                response.ApiPort = details.ApiPort;
                response.Id = details.Id;

                return response;
            }

            response.FailedReason = "out of servers";

            return response;
        }

        [ApiCommand("master/status")]
        public object MasterStatus()
        {
            var response = new MasterStatusResponse();

            response.Success = true;
            
            response.Instances = MasterServer.Instances.Select(i => i.Id).ToList();

            response.CanHostAdditionWorldInstances =
                MasterServer.Instances.Count(i => i.ServerType == ServerType.World) <
                MasterServer.Config.Networking.MaxWorldServers;

            return response;
        }
    }
}