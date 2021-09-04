using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.GnarledForest
{
    [ScriptName("l_pirate_rep")]
    public class PirateRep : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public PirateRep(GameObject gameObject) : base(gameObject)
        {
            if (gameObject.TryGetComponent<MissionGiverComponent>(out var giverComponent))
            Listen(giverComponent.OnMissionOk, vars =>
            {
                if (vars.isComplete && gameObject.Settings.TryGetValue("missionID", out var mission) && mission is int missionId && vars.missionId == missionId && vars.responder is Player player)
                {
                    //what does this flag do?
                    player.GetComponent<CharacterComponent>().SetFlagAsync(24, true);
                }
            });
        }
    }
}