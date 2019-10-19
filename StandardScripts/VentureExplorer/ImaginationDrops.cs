using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting;

namespace StandardScripts.VentureExplorer
{
    [ZoneSpecific(ZoneId.VentureExplorer)]
    public class ImaginationDrops : Script
    {
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects)
            {
                if (gameObject is Player) continue;

                if (!gameObject.TryGetComponent<DestructibleComponent>(out var destructibleComponent)) continue;
                
                destructibleComponent.OnSmashed.AddListener((killer, owner) =>
                {
                    if (owner.GetComponent<Stats>().MaxImagination == default) return;

                    var loot = InstancingUtil.Loot(Lot.Imagination, owner, gameObject, gameObject.Transform.Position);

                    Start(loot);
                });
            }
            
            return Task.CompletedTask;
        }
    }
}