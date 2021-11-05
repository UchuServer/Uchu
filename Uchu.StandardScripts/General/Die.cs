using System;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_lego_die_roll.lua")]
    public class Die : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Die(GameObject gameObject) : base(gameObject)
        {
            var cdClientContext = new CdClientContext();
            var animation = cdClientContext.AnimationsTable.FirstOrDefault(e => e.Animationtype == "dice-roll");
            if (animation?.Animationlength == null) return;
            var length = animation.Animationlength * 1000;
            Zone.Schedule(() => 
            {
                Zone.BroadcastMessage(new DieMessage
                {
                    Associate = gameObject,
                    ClientDeath = true,
                    SpawnLoot = false,
                    DeathType = "",
                    KillType = KillType.Silent,
                    Killer = gameObject,
                    LootOwner = default
                });
                World.Object.Destroy(gameObject);
                //should we add a RequestDie method? this seems a bit excessive to do every time
            }, 10000);
            Zone.Schedule(async () => 
            {
                var rand = new Random();
                var roll = rand.Next(1, 7);
                //roll = 6; 
                //for testing, if your luck is as bad as mine
                gameObject.PlayFX("diceroll", $"Die-Roll-{roll}");
                
                if (gameObject is AuthoredGameObject authored && authored.Author.TryGetComponent<MissionInventoryComponent>(out var missions))
                {
                    string bonusLog = "";
                    if (roll == 6)
                    {
                        bonusLog = "! Congratulations!";
                        var mission = missions.GetMission(756);
                        await missions.ScriptAsync(mission.Tasks[0].TaskId, gameObject.Lot);
                    }
                    Logger.Information($"{authored.Author.Name} rolled a {roll}{bonusLog}");
                }
            }, (float) length);
        }
    }
}
