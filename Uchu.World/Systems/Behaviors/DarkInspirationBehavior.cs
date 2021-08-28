using System.Threading.Tasks;
using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;
using System;
using System.Linq;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class DarkInspirationBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.DarkInspiration;

        private BehaviorBase Action { get; set; }
        private Action<GameObject> OnSmash { get; set; }

        private int Faction { get; set; }

        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
            Faction = await GetParameter<int>("faction_list"); //i don't know how to feel about this
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            OnSmash = (target) => 
            {
                if (target.TryGetComponent<DestroyableComponent>(out var component))
                {
                    if (component.Factions.Contains(Faction))
                    {
                        Action.ExecuteStart(parameters);
                    }
                }
            };
            if (parameters.Context.Associate is Player player)
            {
                player.OnSmashObject.AddListener(OnSmash);
            }
        }

        public override void Dismantle(BehaviorExecutionParameters parameters)
        {
            if (parameters.Context.Associate is Player player)
            {
                player.OnSmashObject.RemoveListener(OnSmash);
            }
        }
    }
}