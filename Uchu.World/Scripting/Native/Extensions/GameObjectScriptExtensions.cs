namespace Uchu.World.Scripting.Native
{
    public static class GameObjectScriptExtensions
    {
        public static void Animate(this GameObject @this, string animation, bool playImmediate = false, float priority = 0.4f)
        {
            @this.Zone.BroadcastMessage(new PlayAnimationMessage
            {
                Associate = @this,
                AnimationId = animation,
                PlayImmediate = playImmediate,
                Priority = priority,
                Scale = 1,
            });
        }

        public static void PlayFX(this GameObject @this, string name, string type, int id = -1, Player excluded = null)
        {
            @this.Zone.ExcludingMessage(new PlayFXEffectMessage
            {
                Associate = @this,
                EffectId = id,
                EffectType = type,
                Name = name,
                Priority = 1,
                Scale = 1,
                Serialize = true,
            }, excluded);
        }

        public static void StopFX(this GameObject @this, string name, bool killImmediate = false, Player excluded = null)
        {
            @this.Zone.ExcludingMessage(new StopFXEffectMessage
            {
                Associate = @this,
                Name = name,
                KillImmediate = killImmediate
            }, excluded);
        }

        public static string[] GetGroups(this GameObject @this)
        {
            if (!@this.Settings.TryGetValue("groupID", out var groupId)) return new string[0];

            if (!(groupId is string groupIdString)) return new string[0];

            var groups = groupIdString.Split(';');

            return groups;
        }
    }
}
