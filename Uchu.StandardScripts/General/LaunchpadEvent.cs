using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World;
using Uchu.World.Scripting;

namespace StandardScripts.General
{
    public class LaunchpadEvent : Script
    {
        public override Task LoadAsync()
        {
            Zone.OnPlayerLoad.AddListener(player =>
            {
                player.OnFireServerEvent.AddListener("ZonePlayer", async message =>
                {
                    var launchpad = message.Associate.GetComponent<RocketLaunchpadComponent>();

                    await using var cdClient = new CdClientContext();

                    var id = launchpad.GameObject.Lot.GetComponentId(ComponentId.RocketLaunchComponent);

                    var launchpadComponent = await cdClient.RocketLaunchpadControlComponentTable.FirstOrDefaultAsync(
                        r => r.Id == id
                    );

                    if (launchpadComponent == default)
                    {
                        return;
                    }

                    await using var ctx = new UchuContext();

                    var character = await ctx.Characters.FirstAsync(c => c.CharacterId == player.ObjectId);

                    character.LaunchedRocketFrom = Zone.ZoneId;

                    await ctx.SaveChangesAsync();

                    if (launchpadComponent.TargetZone != null)
                    {
                        var target = (ZoneId) launchpadComponent.TargetZone;
                        
                        //
                        // We don't want to lock up the server on a world server request, as it may take time.
                        //

                        var _ = Task.Run(async () =>
                        {
                            var success = await player.SendToWorldAsync(target);

                            if (!success)
                            {
                                player.SendChatMessage($"Failed to transfer to {target}, please try later.");
                            }
                        });
                    }
                });
                
                return Task.CompletedTask;
            });
            
            return Task.CompletedTask;
        }
    }
}