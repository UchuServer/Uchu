using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class MonumentBuildableFans : NativeScript
    {
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects)
            {
                if (gameObject.Lot != 4359 && gameObject.Lot != 5904 && gameObject.Lot != 6206) continue;
                
                if (!gameObject.TryGetComponent<QuickBuildComponent>(out var quickBuildComponent)) continue;

                Listen(quickBuildComponent.OnStateChange, (state) =>
                {
                    if (state != RebuildState.Completed) return;
                    
                    gameObject.PlayFX("onbounce", "onbounce", 194);
                    gameObject.PlayFX("create", "create", 200);
                    gameObject.PlayFX("create", "create", 511);
                    gameObject.PlayFX("BrickFadeUpVisCompleteEffect", "create", 507);
                });
            }

            return Task.CompletedTask;
        }
    }
}