using System;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    [ZoneSpecific(1000)]
    public class ShipShake : NativeScript
    {
        private const string ScriptName = "l_ag_ship_shake.lua";

        private Random _random;

        private bool _running = true;
        
        private int RepeatTime { get; set; } = 2;

        private int RandomTime { get; set; } = 10;

        private float ShakeRadius{ get; set; } = 500f;

        private GameObject DebrisObject { get; set; }
        
        private GameObject ShipFxObject { get; set; }
        
        private GameObject ShipFxObject2 { get; set; }

        private string FxName { get; set; } = "camshake-bridge";
        
        private GameObject Self { get; set; }
        
        private int NewTime()
        {
            var time = _random.Next(0, RandomTime + 1);

            if (time < RandomTime / 2)
            {
                time += RandomTime / 2;
            }

            return RandomTime + time;
        }
        
        public override Task LoadAsync()
        {
            Self = GameObject.Instantiate(Zone, 33, new Vector3
            {
                /*
                 * Arbitrary position
                 */
                X = -418,
                Y = 585,
                Z = -30
            });

            Start(Self);

            Construct(Self);
            
            _random = new Random();

            DebrisObject = GetGroup("DebrisFX")[0];
            ShipFxObject = GetGroup("ShipFX")[0];
            ShipFxObject2 = GetGroup("ShipFX2")[0];
            
            Task.Run(async () =>
            {
                await Task.Delay(RepeatTime);

                DoShake(false);
            });
            
            return Task.CompletedTask;
        }

        private void DoShake(bool explodeIdle)
        {
            if (!_running) return;
            
            if (!explodeIdle)
            {
                SetTimer(() =>
                {
                    DoShake(false);
                }, NewTime() * 1000);
                
                Zone.BroadcastMessage(new PlayEmbeddedEffectOnAllClientsNearObjectMessage
                {
                    Associate = Self,
                    Radius = ShakeRadius,
                    EffectName = FxName,
                    FromObject = Self
                });
                
                DebrisObject.PlayFX("Debris", "DebrisFall");

                var randomFx = _random.Next(0, 4);
                
                ShipFxObject.PlayFX("FX", $"shipboom{randomFx}", 559);
                
                SetTimer(() =>
                {
                    DoShake(true);
                }, 5000);

                ShipFxObject2.Animate("explosion");
            }
            else
            {
                ShipFxObject.Animate("idle");
                ShipFxObject2.Animate("idle");
            }
        }

        public override Task UnloadAsync()
        {
            _running = false;

            Destroy(Self);
            
            return Task.CompletedTask;
        }
    }
}