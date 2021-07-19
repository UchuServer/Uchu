using System.Numerics;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
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

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            var quickBuild = GameObject.Instantiate<AuthoredGameObject>(
                parameters.Context.Associate.Zone,
                Lot,
                parameters.Context.Associate.Transform.Position,
                parameters.Context.Associate.Transform.Rotation,
                parameters.Context.Associate
            );

            quickBuild.Transform.Position = parameters.Context.Associate.Transform.Position;

            Object.Start(quickBuild);
            GameObject.Construct(quickBuild);
            GameObject.Serialize(quickBuild);

            var _ = Task.Run(async () =>
            {
                await Task.Delay(parameters.BranchContext.Duration);
                Object.Destroy(quickBuild);
            });
        }
    }
}
