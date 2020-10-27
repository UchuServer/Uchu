using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class SpawnObjectBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.SpawnObject;
        
        private Lot Lot { get; set; }
        private float Distance { get; set; }
        private float ObjectRadius { get; set; }
        private BehaviorBase SpawnActionFail { get; set; }
        private int UpdatePositionWithParent { get; set; }
        
        public override async Task BuildAsync()
        {
            Lot = await GetParameter<int>("LOT_ID");
            Distance = await GetParameter<float>("distance");
            ObjectRadius = await GetParameter<int>("objectRadius");
            SpawnActionFail = await GetBehavior("spawn_fail_action");
            UpdatePositionWithParent = await GetParameter<int>("updatePositionWithParent");
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            var obj = GameObject.Instantiate<AuthoredGameObject>(
                parameters.Context.Associate.Zone, 
                Lot, 
                parameters.Context.Associate.Transform.Position, 
                parameters.Context.Associate.Transform.Rotation
            );

            obj.Author = parameters.Context.Associate;

            Object.Start(obj);
            GameObject.Construct(obj);

            var _ = Task.Run(async () =>
            { 
                await Task.Delay(parameters.BranchContext.Duration); 
                Object.Destroy(obj);
            });
        }
    }
}