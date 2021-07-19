using System;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Core;
using InfectedRose.Lvl;
using Uchu.Physics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/ai/act/l_act_player_death_trigger.lua
    /// </summary>
    [ScriptName("l_act_player_death_trigger.lua")]
    public class DeathPlane : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public DeathPlane(GameObject gameObject) : base(gameObject)
        {
            Console.WriteLine("Death barrier positioned at " + gameObject.Transform.Position);

            var physics = gameObject.GetComponent<PhysicsComponent>();
            if (physics == null || physics == default) return;
            Listen(physics.OnEnter, other =>
            {
                if (!(other.GameObject is Player player)) return;
                Task.Run(async () =>
                {
                    player.GetComponent<DestroyableComponent>().Health = 0;
                });
            });
        }
    }
}