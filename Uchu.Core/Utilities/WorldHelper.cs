using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Uchu.Core
{
    public static class WorldHelper
    {
        public static async Task RequestWorldServerAsync(ZoneId zoneId, Action<int> callback)
        {
            var id = Guid.NewGuid();

            await using (var ctx = new UchuContext())
            {
                await ctx.WorldServerRequests.AddAsync(new WorldServerRequest
                {
                    Id = id,
                    ZoneId = zoneId
                }).ConfigureAwait(false);

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }

            var _ = Task.Run(async () =>
            {
                var timeout = 1000;
                
                while (timeout != default)
                {
                    await using var ctx = new UchuContext();
                    
                    var request = await ctx.WorldServerRequests.FirstAsync(r => r.Id == id).ConfigureAwait(false);

                    if (request.State != WorldServerRequestState.Complete)
                    {
                        timeout--;

                        await Task.Delay(100).ConfigureAwait(false);
                        
                        continue;
                    }

                    Logger.Information($"Request completed {id} {request.SpecificationId}");
                    
                    ctx.WorldServerRequests.Remove(request);

                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                    
                    var specification = await ctx.Specifications.FirstAsync(s => s.Id == request.SpecificationId).ConfigureAwait(false);

                    callback(specification.Port);
                    
                    return;
                }
                
                Logger.Error($"Request {id} timed out");
            });
        }
    }
}