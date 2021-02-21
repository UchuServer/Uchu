using System.Threading.Tasks;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.BlockYard
{
    [ZoneSpecific(1150)]
    public class ModelMania : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.GetComponent<MissionInventoryComponent>().OnAcceptMission, async instance =>
                {
                    if (instance.MissionId == (int) MissionId.ModelMania)
                    {
                        var character = player.GetComponent<CharacterComponent>();
                        await character.SetFlagAsync(FlagId.PlaceModel1, true);
                        await character.SetFlagAsync(FlagId.PlaceModel2, true);
                        await character.SetFlagAsync(FlagId.PlaceModel3, true);
                        await character.SetFlagAsync(FlagId.PlaceModel4, true);
                    }
                });
            });
            
            return Task.CompletedTask;
        }
    }
}