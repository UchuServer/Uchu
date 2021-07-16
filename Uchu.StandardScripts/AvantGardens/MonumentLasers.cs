using System;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{

    /// <summary>
    /// Native implementation of scripts\02_server\map\ag\l_ag_laser_sensor_server.lua
    /// </summary>
    [ScriptName("l_ag_laser_sensor_server.lua")]
    public class MonumentLasers : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public MonumentLasers(GameObject gameObject) : base(gameObject)
        {
            var physics = gameObject.GetComponent<PhysicsComponent>();
            this.SetVar("timeout", false);
            
            // Listen to the physics colliding.
            Listen(physics.OnCollision,  other =>
            {
                if (!(other.GameObject is Player)) return;
                var group = gameObject.GetGroups()[0];
                var shooter = GetVolumeGroup(Zone, group);
                if (shooter.Length == 0) return;
                if (this.GetVar<bool>("timeout")) return;
                
                // Find the laser object.
                GameObject laserObject = default;
                foreach (var zoneGameObject in Zone.GameObjects)
                {
                    if (!zoneGameObject.Settings.ContainsKey("volGroup")) continue;
                    if (((string) zoneGameObject.Settings["volGroup"]).Equals(gameObject.GetGroups()[0])) continue;
                    laserObject = zoneGameObject;
                    break;
                }
                if (laserObject == default) return;
                
                // Perform the laser skill.
                var skillComponent = laserObject.AddComponent<SkillComponent>();
                this.SetVar("timeout", true);
                Task.Run(async () =>
                {
                    try
                    {
                        await skillComponent.CalculateSkillAsync(163, other.GameObject);
                        await Task.Delay(1500);
                        this.SetVar("timeout", false);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                        throw;
                    }
                });
            });
        }
    }
}