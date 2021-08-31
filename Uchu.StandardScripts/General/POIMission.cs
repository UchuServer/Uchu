using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_poi_mission.lua")]
    public class POIMission : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public POIMission(GameObject gameObject) : base(gameObject)
        {
            //this script seems to do literally nothing, since the POI is already checked in the base physicscomponent, but i already wrote some code before i knew that, so i'm leaving it here commented out

            /*
            if (gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent))
            {
                if (gameObject.Settings.TryGetValue("POI", out var obj) && obj is string placeOfInterest)
                Listen(physicsComponent.OnCollision, (collider) => {
                    if (collider.GameObject is Player player && player.TryGetComponent<MissionInventoryComponent>(out var missions))
                    {
                        missions.DiscoverAsync(placeOfInterest);
                    }
                });
            }
            */
        }
    }
}