using System.Numerics;
using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class SpawnQuickbuildBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.SpawnQuickbuild;
        
        public Lot Lot { get; set; }
        
        public float Distance { get; set; }
        
        public float ObjectRadius { get; set; }
        
        public Vector3 Offset { get; set; }
        
        public float RepositionPlayer { get; set; }
        
        public BehaviorBase SpawnFailAction { get; set; }
        
        public override async Task BuildAsync()
        {
            Lot = await GetParameter<int>("LOT_ID");
            Distance = await GetParameter<float>("distance");
            ObjectRadius = await GetParameter<float>("objectRadius");
            
            Offset = new Vector3
            {
                X = await GetParameter<float>("offsetX"),
                Y = await GetParameter<float>("offsetY"),
                Z = await GetParameter<float>("offsetZ")
            };

            RepositionPlayer = await GetParameter<float>("repositionPlayer");

            SpawnFailAction = await GetBehavior("spawn_fail_action");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            var quickBuild = GameObject.Instantiate(
                context.Associate.Zone,
                Lot,
                context.Associate.Transform.Position,
                context.Associate.Transform.Rotation
            );

            quickBuild.Transform.Position = context.Associate.Transform.Position;

            Object.Start(quickBuild);
            GameObject.Construct(quickBuild);
            GameObject.Serialize(quickBuild);

            var _ = Task.Run(async () =>
            {
                await Task.Delay(branchContext.Duration);

                Object.Destroy(quickBuild);
            });
        }
    }
}