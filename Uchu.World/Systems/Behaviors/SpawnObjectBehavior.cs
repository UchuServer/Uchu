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
        
        public override void Build()
        {
            Lot = GetParameter<int>("LOT_ID");
            Distance = GetParameter<float>("distance");
            ObjectRadius = GetParameter<int>("objectRadius");
            SpawnActionFail = GetBehavior("spawn_fail_action");
            UpdatePositionWithParent = GetParameter<int>("updatePositionWithParent");
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

            // Run in the background as long running task as game object construction can lead to game messages
            Task.Run(() =>
            {
                Object.Start(obj);
                GameObject.Construct(obj);
                
                // Schedule the destruction in the game loop
                parameters.Schedule(() => Object.Destroy(obj), parameters.BranchContext.Duration);
            });
        }
    }
}