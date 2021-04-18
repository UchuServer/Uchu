using System.Threading;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class JetpackBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Jetpack;

        private int Airspeed { get; set; }

        private int MaxAirspeed { get; set; }

        private bool EnableHover { get; set; }

        private float VerticalVelocity { get; set; }

        public override async Task BuildAsync()
        {
            // These 0 checks are for the hover jetpack (Lot 7292) which doesn't have the fields (https://lu-explorer.web.app/skills/behaviors/3784)
            var airspeed = await GetParameter<int>("airspeed");
            Airspeed = airspeed > 0 ? airspeed : 20;

            var maxAirspeed = await GetParameter<int>("max_airspeed");
            MaxAirspeed = maxAirspeed > 0 ? maxAirspeed : Airspeed + 10;

            var verticalVelocity = await GetParameter<float>("vertical_velocity");
            VerticalVelocity = verticalVelocity > 0 ? verticalVelocity : 1.5f;

            EnableHover = await GetParameter<bool>("enable_hover");
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            if (!(parameters.Context.Associate is Player player)) return;

            if (!player.TryGetComponent<ControllablePhysicsComponent>(
                out var controllablePhysicsComponent)) return;

            controllablePhysicsComponent.JetpackEffectId = (uint) EffectId;

            player.Zone.BroadcastMessage(new SetJetPackModeMessage
            {
                AirSpeed = Airspeed,
                MaxAirSpeed = MaxAirspeed,
                VerticalVelocity = VerticalVelocity,
                DoHover = EnableHover,
                EffectId = EffectId,
                BypassChecks = true,
                Use = true,
                Associate = player,
            });
        }

        public override void Dismantle(BehaviorExecutionParameters parameters)
        {
            if (!(parameters.Context.Associate is Player player)) return;

            player.Zone.BroadcastMessage(new SetJetPackModeMessage
            {
                AirSpeed = Airspeed,
                MaxAirSpeed = MaxAirspeed,
                VerticalVelocity = VerticalVelocity,
                DoHover = EnableHover,
                EffectId = EffectId,
                BypassChecks = true,
                Use = false,
                Associate = player,
            });
        }
    }
}