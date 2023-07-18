using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("minigame_blue_mark.lua")]
public class MinigameBlueMark : ObjectScript
{
    public MinigameBlueMark(GameObject gameObject) : base(gameObject)
    {
        Listen(gameObject.OnStart, () =>
        {
            Zone.BroadcastMessage(new NotifyClientObjectMessage
            {
                Name = "Blue_Mark"
            });
        });
    }
}