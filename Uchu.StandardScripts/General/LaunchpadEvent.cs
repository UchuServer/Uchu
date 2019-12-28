using System;
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
                        Console.WriteLine(
                            $"{launchpad.GameObject} has an invalid launchpad component: {id}",
                            true
                        );
                        
                        return;
                    }

                    await using var ctx = new UchuContext();

                    var character = await ctx.Characters.FirstAsync(c => c.CharacterId == player.ObjectId);

                    character.LaunchedRocketFrom = Zone.ZoneId;

                    await ctx.SaveChangesAsync();

                    if (launchpadComponent.TargetZone != null)
                    {
                        await player.SendToWorldAsync((ZoneId) launchpadComponent.TargetZone);
                        
                        return;
                    }

                    Console.WriteLine(
                        $"{launchpad.GameObject} has an invalid launchpad target zone: {launchpadComponent.TargetZone}"
                    );
                });
                
                return Task.CompletedTask;
            });
            
            return Task.CompletedTask;
        }
    }
}