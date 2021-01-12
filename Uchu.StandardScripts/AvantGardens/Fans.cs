using System;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class Fans : NativeScript
    {
        public override Task LoadAsync()
        {
            var ids = new[]
            {
                (19, 1),
                (19, 2),
                (20, 1)
            };

            foreach (var trigger in GetTriggers(ids))
            {
                var gameObject = trigger.GameObject;
                
                if (!gameObject.TryGetComponent<SwitchComponent>(out var switchComponent)) continue;
                
                gameObject.TryGetComponent<QuickBuildComponent>(out var buttonQuickBuild);

                var fanId = trigger.Trigger.Events[0].Commands[0].TargetName;

                GameObject fanObject = default;

                foreach (var zoneGameObject in Zone.GameObjects)
                {
                    //if (!zoneGameObject.TryGetComponent<LuaScriptComponent>(out _)) continue;
                    if (!zoneGameObject.Settings.TryGetValue("groupID", out var groupObj)) continue;

                    var groupId = (string) groupObj;

                    if (groupId != $"{fanId};") continue;
                    
                    fanObject = zoneGameObject;
                    
                    break;
                }
                
                Listen(switchComponent.OnActivated, player =>
                {
                    ActivateFx(fanObject);
                    
                    return Task.CompletedTask;
                });
                
                Listen(switchComponent.OnDeactivated, () =>
                {
                    DeactivateFx(fanObject);
                    
                    return Task.CompletedTask;
                });
                
                ActivateFx(fanObject);
                
                Listen(Zone.OnTick, () =>
                {
                    if (switchComponent.State) return Task.CompletedTask;
                    
                    foreach (var player in Zone.Players)
                    {
                        if (player?.Transform == default) return Task.CompletedTask;
                        
                        if (buttonQuickBuild != default && buttonQuickBuild.State != RebuildState.Completed) return Task.CompletedTask;
                        
                        if (!(Vector3.Distance(player.Transform.Position, gameObject.Transform.Position) < 2)) continue;

                        switchComponent.Activate(player);

                        Serialize(gameObject);
                    }
                    
                    return Task.CompletedTask;
                });
            }
            
            return Task.CompletedTask;
        }

        private void ActivateFx(GameObject gameObject)
        {
            Console.WriteLine("Active");
            
            var group = (gameObject.Settings["groupID"] as string)?.Split(';')[0];
            
            if (group == default) return;

            gameObject.Animate("fan-off", true);

            gameObject.StopFX("fanOn");

            foreach (var fanObject in GetGroup(group))
            {
                // Nothing is found here
                if (!fanObject.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) continue;
                    
                physicsComponent.IsEffectActive = false;
                
                Serialize(fanObject);
            }

            GetGroup($"{group}fx")[0].Animate("trigger", true);
        }

        private void DeactivateFx(GameObject gameObject)
        {
            Console.WriteLine("Deactivated");
            
            var group = (gameObject.Settings["groupID"] as string)?.Split(';')[0];
            
            if (group == default) return;

            gameObject.Animate("fan-on", true);

            gameObject.PlayFX("fanOn", "fanOn", 495);

            foreach (var fanObject in GetGroup(group))
            {
                // Nothing is found here
                if (!fanObject.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) continue;
                
                physicsComponent.IsEffectActive = true;

                Serialize(fanObject);
            }

            GetGroup($"{group}fx")[0].Animate("idle", true);
        }
    }
}