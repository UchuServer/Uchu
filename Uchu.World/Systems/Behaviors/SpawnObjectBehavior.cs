using System.Numerics;
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
            var rotateOffset = Vector3.Transform(new Vector3(0, 0, Distance), parameters.Context.Associate.Transform.Rotation);
            var obj = GameObject.Instantiate<AuthoredGameObject>(
                parameters.Context.Associate.Zone, 
                Lot, 
                parameters.Context.Associate.Transform.Position + rotateOffset, 
                parameters.Context.Associate.Transform.Rotation,
                parameters.Context.Associate
            );

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