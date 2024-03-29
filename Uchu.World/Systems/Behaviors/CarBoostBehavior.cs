using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class CarBoostBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.CarBoost;

        // Action to execute on success
        private BehaviorBase Action;
        // Action to execute when boost failed
        private BehaviorBase ActionFailed;
        // Time of boost
        private float Time;

        public override async Task BuildAsync()
        {
            Action = await GetBehavior(await GetParameter<uint>("action"));
            Time = await GetParameter<float>("time");
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            if (!(parameters.Context.Associate is Player player))
                return;
            
            Action.ExecuteStart(parameters);

            player.Zone.ExcludingMessage(new VehicleAddPassiveBoostAction
            {
                Associate = parameters.BranchContext.Target,
            }, player);
            
            player.Zone.Schedule(() =>
            {
                player.Zone.ExcludingMessage(new VehicleRemovePassiveBoostAction
                {
                    Associate = parameters.BranchContext.Target,
                }, player);
            }, Time * 1000);
        }
    }
}
