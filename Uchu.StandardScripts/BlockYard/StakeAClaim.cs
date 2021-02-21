using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.BlockYard
{
    [ZoneSpecific(1150)]
    public class StakeAClaim : NativeScript
    {
        public override Task LoadAsync()
        {
            if (Zone.GameObjects.FirstOrDefault(gameObject => gameObject.Lot == Lot.PropertyPlagueVendor) 
                    is {} propertyPlague)
            {
                HandlePropertyPlagueInteraction(propertyPlague);
            }
            else
            {
                Listen(Zone.OnObject, o =>
                {
                    if (o is GameObject gameObject && gameObject.Lot == Lot.PropertyPlagueVendor)
                    {
                        HandlePropertyPlagueInteraction(gameObject);
                    }
                });
            }

            return Task.CompletedTask;
        }

        private void HandlePropertyPlagueInteraction(GameObject propertyPlage)
        {
            Listen(propertyPlage.OnInteract, player =>
            {
                player.GetComponent<CharacterComponent>().SetFlagAsync(FlagId.ClaimBlockyardProperty, true);
            });
        }
    }
}