using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class SpawnObjectBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.SpawnObject;
        
        public Lot Lot { get; set; }
        
        public float Distance { get; set; }
        
        public float ObjectRadius { get; set; }
        
        public BehaviorBase SpawnActionFail { get; set; }
        
        public int UpdatePositionWithParent { get; set; }
        
        public override async Task BuildAsync()
        {
            Lot = await GetParameter<int>("LOT_ID");
            Distance = await GetParameter<float>("distance");
            ObjectRadius = await GetParameter<int>("objectRadius");

            SpawnActionFail = await GetBehavior("spawn_fail_action");
            UpdatePositionWithParent = await GetParameter<int>("updatePositionWithParent");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
             await base.ExecuteAsync(context, branchContext);

             var obj = GameObject.Instantiate<AuthoredGameObject>(
                 context.Associate.Zone,
                 Lot,
                 context.Associate.Transform.Position,
                 context.Associate.Transform.Rotation
             );

             obj.Author = context.Associate;

             Object.Start(obj);
             GameObject.Construct(obj);

             var _ = Task.Run(async () =>
             {
                 await Task.Delay(branchContext.Duration);

                 Object.Destroy(obj);
             });
        }
    }
}