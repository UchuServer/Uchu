using System.Linq;
using Uchu.Core;

namespace Uchu.World.Handlers.Commands
{
    public class PropHunt : HandlerGroup
    {
        [CommandHandler(Signature = "prop", Help = "Hide as prop", GameMasterLevel = GameMasterLevel.Player)]
        public string Prop(string[] arguments, Player player)
        {
            if (player.Zone.GameObjects.OfType<MaskObject>().Any(markObject => markObject.Author == player))
            {
                return "You are already hiding!";
            }

            if (arguments.Length == default)
            {
                return "prop <lot>";
            }

            if (!int.TryParse(arguments[0], out var lot))
            {
                return "Invalid <lot>";
            }

            var maskObject = GameObject.Instantiate<MaskObject>(
                player.Zone,
                lot,
                player.Transform.Position,
                player.Transform.Rotation
            );

            maskObject.Author = player;

            Object.Start(maskObject);
            GameObject.Construct(maskObject);

            return $"Hid as {lot}";
        }

        [CommandHandler(Signature = "reveal", Help = "Reveal yourself", GameMasterLevel = GameMasterLevel.Player)]
        public string Reveal(string[] arguments, Player player)
        {
            foreach (var markObject in player.Zone.GameObjects.OfType<MaskObject>())
            {
                if (markObject.Author != player) continue;
                
                Object.Destroy(markObject);

                return "Revealed yourself!";
            }

            return "You were not hiding!";
        }
    }
}