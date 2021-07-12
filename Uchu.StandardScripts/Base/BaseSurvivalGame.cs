using Uchu.World;

namespace Uchu.StandardScripts.Base
{
    /// <summary>
    /// Native implementation of scripts/ai/minigame/survival/base_survival_server.lua
    /// </summary>
    public class BaseSurvivalGame : GenericActivityManager
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public BaseSurvivalGame(GameObject gameObject) : base(gameObject)
        {
            
        }
    }
}