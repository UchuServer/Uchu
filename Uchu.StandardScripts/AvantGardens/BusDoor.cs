using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Native implementation of scripts/ai/ag/l_ag_bus_door.lua
    /// </summary>
    [ScriptName("l_ag_bus_door.lua")]
    public class BusDoor : ObjectScript
    {
        /// <summary>
        /// Inner proximity for opening the bus door.
        /// </summary>
        public const float InnerProximityRadius = 75;
        
        /// <summary>
        /// Outer proximity for closing the bus door.
        /// </summary>
        public const float OuterProximityRadius = 85;
        
        /// <summary>
        /// GUID for the sound when moving the door.
        /// </summary>
        public const string SoundName = "{9a24f1fa-3177-4745-a2df-fbd996d6e1e3}";

        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public BusDoor(GameObject gameObject) : base(gameObject)
        {
            // Set the counters.
            this.SetVar("InnerCounter", 0);
            this.SetVar("OuterCounter", 0);
            
            // Set the proximity for opening and closing the door.
            this.SetProximityRadius(InnerProximityRadius, "BusDoorInner");
            this.SetProximityRadius(OuterProximityRadius, "BusDoorOuter");
            
            // Stop the automatic platform movement.
            this.StopPathing();
        }

        /// <summary>
        /// Moves the bus door to a given position.
        /// </summary>
        /// <param name="open">Whether the door should be open.</param>
        public void MoveDoor(bool open)
        {
            // Move the door to the desired waypoint.
            if (open)
            {
                this.GoToWaypoint(0);
            }
            else
            {
                this.GoToWaypoint(1);
            }
            
            // Play the moving audio.
            this.PlayNDAudioEmitter(SoundName);
        }

        /// <summary>
        /// Callback for the proximity of a player updating (entering
        /// or leaving a physics body).
        /// </summary>
        /// <param name="name">Name of the physics body.</param>
        /// <param name="status">Status of the player.</param>
        /// <param name="player">Player that changed.</param>
        public override void OnProximityUpdate(string name, PhysicsCollisionStatus status, Player player)
        {
            // Return if the name isn't correct.
            if (name != "BusDoorInner" && name != "BusDoorOuter")
            {
                return;
            }
            
            // Update the counters and move the door.
            var innerCounter = this.GetVar<int>("InnerCounter");
            var outerCounter = this.GetVar<int>("OuterCounter");
            if (status == PhysicsCollisionStatus.Enter)
            {
                // Add the counter.
                if (name == "BusDoorInner")
                {
                    innerCounter += 1;
                }
                else
                {
                    outerCounter += 1;
                }
                
                // Open the door if the first player entered the inner region.
                if (innerCounter == 1 && outerCounter >= 1)
                {
                    this.MoveDoor(true);
                }
            } else if (status == PhysicsCollisionStatus.Leave)
            {
                // Subtract the counter.
                if (name == "BusDoorInner")
                {
                    innerCounter += -1;
                }
                else
                {
                    outerCounter += -1;
                }
                
                // Close the door if all players left.
                if (innerCounter == 0 && outerCounter == 0)
                {
                    this.MoveDoor(false);
                }
            }
            
            // Store the counters.
            this.SetVar("InnerCounter", innerCounter);
            this.SetVar("OuterCounter", outerCounter);
        }
        
        /// <summary>
        /// Callback for the path index being reached.
        /// </summary>
        /// <param name="pathIndex">Index of the path reached.</param>
        public override void OnArrivedAtDesiredWaypoint(uint pathIndex)
        {
            // Play the dust effect if the door reached the ground.
            if (pathIndex == 1)
            {
                this.PlayFXEffect("busDust", "create");
            }
        }
    }
}