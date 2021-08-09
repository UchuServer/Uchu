using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/equipmentscripts/spawnpowerupsontimerthendie.lua
    /// </summary>
    public class SpawnPowerupsOnTimerThenDie : ObjectScript
    {
        public SpawnPowerupsOnTimerThenDie(GameObject gameObject) : base(gameObject)
        {
        }
        protected async void SpawnPowerups(GameObject gameObject, int numCycles, float secPerCycle, float delayToFirstCycle, float deathDelay, int numberOfPowerups, Lot lootLOT)
        {
            await Task.Delay((int) (delayToFirstCycle * 1000));
            for (var i = 0; i < numCycles; i++)
            {
                //spawn the powerups
                for (var h = 0; h < numberOfPowerups; h++)
                {
                    foreach (var player in gameObject.Viewers)
                    {
                        var loot = InstancingUtilities.InstantiateLoot(lootLOT, player, gameObject,
                        gameObject.Transform.Position + Vector3.UnitY);
                        Start(loot);
                    }
                }
                await Task.Delay((int) (secPerCycle * 1000));
            }
            await Task.Delay((int) (deathDelay * 1000));
            gameObject.GetComponent<DestructibleComponent>().SmashAsync(gameObject);
        }
    }
}