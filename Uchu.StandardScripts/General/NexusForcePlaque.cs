using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    ///     LUA Reference: l_story_box_interact_client.lua
    /// </summary>
    public class NexusForcePlaque : Script
    {
        private const string ScriptName = "l_story_box_interact_client.lua";
        
        public override Task LoadAsync()
        {
            foreach (var gameObject in HasLuaScript(ScriptName, true))
            {
                if (!gameObject.Settings.TryGetValue("storyText", out var text)) continue;

                var idString = (string) text;
                
                var id = int.Parse(idString.Substring(idString.Length - 2));

                var flag = 10000 + (int) Zone.ZoneId + id;
                
                gameObject.OnInteract.AddListener(player =>
                {
                    player.SendChatMessage($"PLQ: {flag}");
                    
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