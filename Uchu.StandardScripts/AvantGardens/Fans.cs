using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;
using DestructibleComponent = Uchu.World.DestructibleComponent;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Native implementation of scripts/ai/ag/l_ag_fans.lua
    /// </summary>
    [ScriptName("l_ag_fans.lua")]
    public class Fans : ObjectScript
    {
        /// <summary>
        /// Triggers (switches) for the fans.
        /// </summary>
        private static readonly (int, int)[] FanTriggers = new[]
        {
            (19, 1),
            (19, 2),
            (20, 1)
        };

        /// <summary>
        /// Group that the fan is part of.
        /// </summary>
        private string _fanGroup;
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Fans(GameObject gameObject) : base(gameObject)
        {
            // Set the initial variables.
            this.SetVar("IsOn", false);
            
            // Toggle the initial effects.
            this.ToggleFX();
            
            // Connect the switch.
            this._fanGroup = ((string) this.GameObject.Settings["groupID"]).Replace(";", "");
            var switchTrigger = this.GetTriggers(FanTriggers).FirstOrDefault(trigger => 
                trigger.Trigger.Events[0].Commands[0].TargetName == this._fanGroup);
            if (switchTrigger == null) return;
            var switchObject = switchTrigger.GameObject;
            if (!switchObject.TryGetComponent<SwitchComponent>(out var switchComponent)) return;
            Listen(switchComponent.OnActivated, player =>
            {
                this.SetVar("IsOn", true);
                this.ToggleFX();
            });
                
            Listen(switchComponent.OnDeactivated, () =>
            {
                this.SetVar("IsOn", false);
                this.ToggleFX();
            });
        }

        /// <summary>
        /// Toggles the effects.
        /// </summary>
        public void ToggleFX()
        {
            // Get the groups.
            var volumeGroup = this.GetGroup(this._fanGroup);
            var fxGroup = this.GetGroup(this._fanGroup + " fx");
            this.GameObject.TryGetComponent<DestructibleComponent>(out var fanDestructibleComponent);
            var alive = fanDestructibleComponent?.Alive ?? false;
            
            // Update the effects.
            if (this.GetVar<bool>("IsOn") || !alive)
            {
                this.PlayAnimation("fan-off", true);
                this.StopFXEffect("fanOn");
                foreach (var volume in volumeGroup)
                {
                    if (!volume.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) continue;
                    physicsComponent.IsEffectActive = false;
                    Serialize(volume);
                }
                foreach (var fxObject in fxGroup)
                {
                    fxObject.Animate("trigger", true);
                }
            }
            else
            {
                this.PlayAnimation("fan-on", true);
                this.StartFXEffect("fanOn", "fanOn", 495);
                foreach (var volume in volumeGroup)
                {
                    if (!volume.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) continue;
                    physicsComponent.IsEffectActive = true;
                    Serialize(volume);
                }
                foreach (var fxObject in fxGroup)
                {
                    fxObject.Animate("idle", true);
                }
            }
        }

        /// <summary>
        /// Unloads the script.
        /// </summary>
        public override Task UnloadAsync()
        {
            this.ToggleFX();
            return Task.CompletedTask;
        }
    }
}