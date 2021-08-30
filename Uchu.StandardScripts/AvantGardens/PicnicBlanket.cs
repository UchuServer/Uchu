using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ScriptName("l_ag_picnic_blanket.lua")]
    public class PicnicBlanket : ObjectScript
    {
        private bool Active = false;
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public PicnicBlanket(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnInteract, async player =>
            {
                if (player.TryGetComponent<CharacterComponent>(out var character) && !Active)
                {
                    Active = true;
                    for (var i = 0; i < 3; i++)
                    {
                        var loot = InstancingUtilities.InstantiateLoot(935, player, gameObject,
                        gameObject.Transform.Position + Vector3.UnitY);
                        Start(loot);
                    }
                    await Task.Delay(5000);
                    Active = false;
                }
            });
        }
    }
}