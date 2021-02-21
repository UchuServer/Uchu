using System.Threading.Tasks;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.BlockYard
{
    [ZoneSpecific(1150)]
    public class ModelMovers : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.GetComponent<MissionInventoryComponent>().OnAcceptMission, async instance =>
                {
                    if (instance.MissionId == (int) MissionId.ModelMovers)
                    {
                        var character = player.GetComponent<CharacterComponent>();
                        await character.SetFlagAsync(FlagId.PickUpModel, true);
                        await character.SetFlagAsync(FlagId.RotateModel, true);
                        await character.SetFlagAsync(FlagId.PutAwayModel, true);
                    }
                });
            });
            
            return Task.CompletedTask;
        }
    }
}