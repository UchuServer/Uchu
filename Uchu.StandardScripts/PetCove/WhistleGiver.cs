using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
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
                Listen(player.OnFireServerEvent, (s, message) =>
                {
                    if (message.Arguments == "unlockEmote" && player.TryGetComponent<CharacterComponent>(out var character))
                    {
                        character.AddEmote(115);
                    }
                });
            });

            return Task.CompletedTask;
        }
    }
}