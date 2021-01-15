using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.PetCove
{
    [ZoneSpecific(1201)]
    public class WhistleGiver : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnFireServerEvent, async (s, message) =>
                {
                    await using var context = new UchuContext();
                    if (message.Arguments == "unlockEmote")
                    {
                        await player.UnlockEmoteAsync(context, 115);
                    }
                    await context.SaveChangesAsync();
                });
            });
            
            
            return Task.CompletedTask;
        }
    }
}