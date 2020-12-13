using System;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World.Scripting.Lua;

namespace Uchu.World.Scripts.AvantGardens
{
    public class Fans : LuaNativeScript
    {
        private bool IsOn = false;
        private bool IsAlive = true;

        public override Task LoadAsync(GameObject self)
        {
            ToggleFX(self);

            Listen(self.GetComponent<DestroyableComponent>().OnDeath, () =>
            {
                if (IsOn) ToggleFX(self);
                IsAlive = false;
            });

            Listen(self.OnFireServerEvent, (args, message) =>
            {
                if (args.Length == 0 || !IsAlive) return;
                var first = args.Split(',')[0];
                if ((first == "turnOn" && IsOn) || (first == "turnOff" && !IsOn)) return;
                ToggleFX(self);
            });

            return Task.CompletedTask;
        }

        private void ToggleFX(GameObject self, bool IsHit = false)
        {
            if (IsOn)
            {
                var group = (self.Settings["groupID"] as string)?.Split(';')[0];
            
                if (group == default) return;

                self.Animate("fan-off", true);

                self.StopFX("fanOn");

                foreach (var fanObject in GetGroup(group))
                {
                    // Nothing is found here
                    if (!fanObject.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) continue;
                
                    Serialize(fanObject);
                    
                    physicsComponent.IsEffectActive = false;
                    IsOn = false;
                }

                GetGroup($"{group}fx")[0].Animate("trigger", true);
            } 
            else if (!IsOn && IsAlive)
            {
                var group = (self.Settings["groupID"] as string)?.Split(';')[0];
            
                if (group == default) return;

                self.Animate("fan-on", true);

                self.PlayFX("fanOn", "fanOn", 495);

                foreach (var fanObject in GetGroup(group))
                {
                    // Nothing is found here
                    if (!fanObject.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) continue;
                
                    physicsComponent.IsEffectActive = true;
                    IsOn = true;

                    Serialize(fanObject);
                }

                GetGroup($"{group}fx")[0].Animate("idle", true);
            }
        }

        public override string ScriptName { get; set; } = "L_AG_FANS.lua";
    }
}