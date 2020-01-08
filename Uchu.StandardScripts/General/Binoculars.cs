using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    ///     LUA Reference: l_binoculars_client.lua
    /// </summary>
    public class Binoculars : Script
    {
        private const string ScriptName = "l_binoculars_client.lua";
        
        public override Task LoadAsync()
        {
            foreach (var gameObject in HasLuaScript(ScriptName, true))
            {
                if (!gameObject.Settings.TryGetValue("number", out var number)) continue;

                var worldId = ((int) Zone.ZoneId).ToString().Substring(0, 2);

                var flag = int.Parse($"{worldId}{number}");
                
                Listen(gameObject.OnInteract, player =>
                {
                    player.SendChatMessage($"BIO: {flag}");
                    
                    player.GetComponent<MissionInventoryComponent>().UpdateObjectTask(
                        MissionTaskType.Flag,
                        flag,
                        gameObject
                    );
                    
                    player.Message(new NotifyClientFlagChangeMessage
                    {
                        Associate = player,
                        Flag = true,
                        FlagId = flag
                    });
                    
                    player.Message(new FireClientEventMessage
                    {
                        Associate = gameObject,
                        Arguments = "achieve",
                        Target = gameObject,
                        Sender = player
                    });
                });
            }

            return Task.CompletedTask;
        }
    }
}