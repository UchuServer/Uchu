using System.Threading.Tasks;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.BlockYard
{
    [ZoneSpecific(1150)]
    public class BlockYardWarning : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, async player =>
            {
                await UiHelper.AnnouncementAsync(player, "Not yet implemented",
                    "Block Yard is currently not implemented. You can return to Avant Gardens by sending <font color=\"#FF7F00\">/testmap 1100</font> in the chat.");
            });
            throw new System.NotImplementedException();
        }
    }
}
