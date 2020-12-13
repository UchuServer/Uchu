using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class MonumentLasers : NativeScript
    {
        public override Task LoadAsync()
        {
            var lasers = new List<GameObject>();
            
            lasers.AddRange(GetGroup("laser1"));
            lasers.AddRange(GetGroup("laser2"));
            lasers.AddRange(GetGroup("laser3"));
            lasers.AddRange(GetGroup("laser4"));
            

            foreach (var laser in lasers)
            {
                var physics = laser.GetComponent<PhysicsComponent>();
                
                laser.Settings["timeout"] = false;
                
                Listen(physics.OnCollision,  other =>
                {
                    if (!(other.GameObject is Player)) return;

                    var group = laser.GetGroups()[0];

                    var shooter = GetVolumeGroup(Zone, group);
                    
                    if (shooter.Length == 0) return;
                    
                    if ((bool) laser.Settings["timeout"]) return;
                    
                    var skillComponent = laser.AddComponent<SkillComponent>();

                    laser.Settings["timeout"] = true;
                    
                    var _ = Task.Run(async () =>
                    {
                        try
                        {
                            await skillComponent.CalculateSkillAsync(163, other.GameObject);

                            await Task.Delay(1500);

                            laser.Settings["timeout"] = false;
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                            throw;
                        }
                    });
                });
            }
            
            return Task.CompletedTask;
        }
    }
}