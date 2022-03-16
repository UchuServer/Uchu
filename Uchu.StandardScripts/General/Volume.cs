using Uchu.World;
using Uchu.World.Scripting.Native;
using System.Numerics;
using Uchu.Core;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_force_volume_server.lua")]
    [ScriptName("l_friction_volume_server.lua")]
    public class Volume : ObjectScript
    {
        public Volume(GameObject gameObject) : base(gameObject)
        {
            //TODO: replace this script with a check similar to how POI objects are done
            var s = GameObject.Settings;
            if (!gameObject.TryGetComponent<PhantomPhysicsComponent>(out var physics))
                if (gameObject.Settings.ContainsKey("primitiveModelType"))
                    physics = gameObject.AddComponent<PhantomPhysicsComponent>();
                else return;
            //ForceVolumeServer
            if (s.TryGetValue("ForceAmt", out var amt) 
                && s.TryGetValue("ForceX", out var x) 
                && s.TryGetValue("ForceY", out var y) 
                && s.TryGetValue("ForceZ", out var z))
            {
                physics.EffectAmount = (float) amt;
                physics.IsEffectActive = true;
                physics.EffectDirection = new Vector3((float) x, (float) y, (float) z);
                physics.EffectType = PhantomPhysicsEffectType.Push;
                Logger.Log($"Push Volume, LOT: {GameObject.Lot} Amount: {physics.EffectAmount} Direction: {physics.EffectDirection}");
            }
            //FrictionVolumeServer
            if (s.TryGetValue("FrictionAmt", out var friction))
            {
                physics.EffectAmount = (float) friction;
                physics.EffectType = PhantomPhysicsEffectType.Friction;
                physics.IsEffectActive = true;
                Logger.Log($"Friction Volume, LOT: {GameObject.Lot} Amount: {physics.EffectAmount}");
            }
            Serialize(gameObject);
        }
    }
}