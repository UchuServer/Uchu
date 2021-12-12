using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.GnarledForest
{
    [ScriptName("l_gf_banana.lua")]
    public class BananaTree : ObjectScript
    {
        private GameObject Banana { get; set; }
        private bool TimerActive { get; set; }
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public BananaTree(GameObject gameObject) : base(gameObject)
        {
            SpawnBanana();

            var destroyableComponent = gameObject.GetComponent<DestroyableComponent>();

            Listen(destroyableComponent.OnHealthChanged, (newHealth, delta) =>
            {
                if (delta < 0)
                {
                    destroyableComponent.Health = 9999;
                    if (Banana == null || !Banana.Alive || !Banana.TryGetComponent<DestructibleComponent>(out var destructible) || Banana.Transform.Position.Y != gameObject.Transform.Position.Y + 12)
                    {
                        return;
                    }
                    animateBananaSmash();
                }
            });
        }
        private void SpawnBanana()
        {
            Vector3 oPos = GameObject.Transform.Position;
            Quaternion oRot = GameObject.Transform.Rotation;
            oPos.Y += 12;
            oPos.X -= Vector3.Transform(Vector3.UnitX, oRot).X * 5;
            oPos.Z -= Vector3.Transform(Vector3.UnitX, oRot).Z * 5;
            var obj = GameObject.Instantiate<GameObject>(
                GameObject.Zone,
                6909, //banana bunch 
                oPos,
                oRot
            );
            Uchu.World.Object.Start(obj);
            GameObject.Construct(obj);

            Banana = obj;

            Listen(Banana.GetComponent<DestroyableComponent>().OnDeath, () =>
            {
                CancelTimer("smashGroundedBanana");
                AddTimerWithCancel(30, "bananaTimer");
                Banana = null;
            });
        }
        public override void OnTimerDone(string timerName)
        {
            if (timerName == "bananaTimer")
            {
                SpawnBanana();
            }
            else if (timerName == "smashGroundedBanana" && Banana != null && Banana.Alive)
            {
                Banana.GetComponent<DestructibleComponent>().SmashAsync(GameObject);
            }
        }
        private float delta(float first, float second)
        {
            return (cosMethod(first) - cosMethod(second)) * 12;
        }
        private float cosMethod(float value)
        {
            //cos(pi/2) = 0
            return (float)Math.Cos(value * Math.PI);
        }
        private void animateBananaSmash()
        {
            int framerate = 20;
            //var destroyable = Banana.GetComponent<DestroyableComponent>();
            //destroyable.Health = 9999;
            
            //:)
            //this isn't part of the original script but i wanted to make it look nicer
            Task.Run(async () =>
            {
                for (int i = 0; i < framerate / 2; i++)
                {
                    await Task.Delay(1000 / framerate);
                    if (Banana == null || !Banana.Alive) return;
                    Banana.Transform.Translate(new Vector3(0, delta((((float)i + 1) / framerate), (float)i / framerate), 0));
                }
                AddTimerWithCancel(100, "smashGroundedBanana");
                //destroyable.Health = 1;
            });
        }
    }
}