using System;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    ///     LUA Reference: l_ag_space_stuff.lua
    /// </summary>
    [ZoneSpecific(ZoneId.VentureExplorer)]
    public class SpaceStuff : NativeScript
    {
        private const string ScriptName = "l_ag_space_stuff.lua";

        private Random _random;

        private bool _running = true;
        
        public override Task LoadAsync()
        {
            _random = new Random();
            
            foreach (var gameObject in HasLuaScript(ScriptName))
            {
                Task.Run(async () =>
                {
                    await DoSpaceStuff(gameObject);
                });
            }

            return Task.CompletedTask;
        }

        private async Task DoSpaceStuff(GameObject gameObject)
        {
            var path = false;
            
            while (_running)
            {
                if (path)
                {
                    var pathType = _random.Next(1, 5);
                    var randomTime = _random.Next(20, 26);

                    gameObject.Animate($"path_0{pathType}");

                    await Task.Delay(randomTime * 1000);
                    
                    path = false;
                }
                else
                {
                    var scaleType = _random.Next(1, 6);
                    
                    gameObject.Animate($"scale_0{scaleType}");

                    await Task.Delay(400);

                    path = true;
                }
            }
        }

        public override Task UnloadAsync()
        {
            _running = false;

            return Task.CompletedTask;
        }
    }
}