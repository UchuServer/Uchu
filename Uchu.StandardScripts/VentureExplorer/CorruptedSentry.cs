using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    [ScriptName("ScriptComponent_1075_script_name__removed")]
    [LotSpecific(Lot.CruxPrimeMech)]
    [LotSpecific(Lot.CruxPrimeNamedMech)]
    public class CorruptedSentry : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public CorruptedSentry(GameObject gameObject) : base(gameObject)
        {
            // For some reason, corrupted sentries do not have the factions
            // and enemies correctly, which leads to the sentries not firing at
            // players or them taking damage from players.
            if (!gameObject.TryGetComponent<DestroyableComponent>(out var destroyableComponent)) return;
            destroyableComponent.Factions = new[] {4};
            destroyableComponent.Enemies = new[] {1};
        }
    }
}