using System.Threading.Tasks;
using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;
using System;
using System.Linq;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class DarkInspirationExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters Parameters { get; set; }

        public DarkInspirationExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class DarkInspirationBehavior : BehaviorBase<DarkInspirationExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.DarkInspiration;

        private BehaviorBase Action { get; set; }
        private Action<GameObject> OnSmash { get; set; }

        private int Faction { get; set; }

        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
            Faction = await GetParameter<int>("faction_list"); //this is a list, but is being interpreted as just an int, but it's usually only one number
        }

        protected override void ExecuteStart(DarkInspirationExecutionParameters parameters)
        {
            OnSmash = (target) => 
            {
                if (target.TryGetComponent<DestroyableComponent>(out var component))
                {
                    if (component.Factions.Contains(Faction))
                    {
                        Action.ExecuteStart(parameters.Parameters);
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
        protected override void SerializeStart(BitWriter writer, DarkInspirationExecutionParameters parameters)
        {
            parameters.Parameters = Action.SerializeStart(writer, parameters.NpcContext, parameters.BranchContext);
        }
        protected override void DeserializeStart(BitReader reader, DarkInspirationExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(reader, parameters.Context, parameters.BranchContext);
        }
    }
}