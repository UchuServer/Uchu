using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("ScriptComponent_1330_script_name__removed")]
public class ForbiddenValleyColumns : ObjectScript
{
    public ForbiddenValleyColumns(GameObject gameObject) : base(gameObject)
    {
        var movingPlatformComponent = gameObject.GetComponent<MovingPlatformComponent>();

        movingPlatformComponent.MoveTo(0);
    }
}
