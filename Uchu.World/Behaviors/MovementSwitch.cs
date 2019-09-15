using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class MovementSwitch : Behavior
    {
        public MovementType MovementType { get; private set; }

        public override BehaviorTemplateId Id => BehaviorTemplateId.MovementSwitch;

        public override async Task Serialize(BitReader reader)
        {
            MovementType = (MovementType) reader.Read<uint>();

            string name;

            switch (MovementType)
            {
                case MovementType.Ground:
                    name = "ground_action";
                    break;
                case MovementType.Jump:
                    name = "jump_action";
                    break;
                case MovementType.Falling:
                    name = "falling_action";
                    break;
                case MovementType.DoubleJump:
                    name = "double_jump_action";
                    break;
                case MovementType.Jetpack:
                    name = "jetpack_action";
                    break;
                default:
                    name = "";
                    break;
            }

            var param = await GetParameter(BehaviorId, name);

            if (param?.Value == null) return;

            await StartBranch((int) param.Value, reader);
        }
    }
}