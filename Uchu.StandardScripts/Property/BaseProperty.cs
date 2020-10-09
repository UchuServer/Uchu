using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.Core.Resources;
using Uchu.Core;
using System.Configuration;

namespace Uchu.StandardScripts.Property
{
    [ZoneSpecific(1150)]
    public class BaseProperty : NativeScript
    {
        string GUIDMaelstrom { get; set; } = "{7881e0a1-ef6d-420c-8040-f59994aa3357}"; // ambient sounds for when the Maelstrom is on
        string GUIDPeacful { get; set; } = "{c5725665-58d0-465f-9e11-aeb1d21842ba}"; // happy ambient sounds when no Maelstrom is present
        bool OwnerChecked = false;
        ObjectId PropertyOwner = new ObjectId(0);
        public override Task LoadAsync() 
        {
            Listen(Zone.OnPlayerLoad, (player) => {
                bool Rented = false;
                if (!OwnerChecked)
                {
                    CheckForOwner();
                }

                if ((ulong)PropertyOwner != 0)
                {
                    Rented = true;
                }

                if (Rented)
                {
                    player.Message(new PlayNDAudioEmitterMessage
                    {
                        Associate = player,
                        NDAudioEventGUID = GUIDPeacful
                    });
                }
            });

            return Task.CompletedTask; 
        }

        private bool CheckForOwner()
        {
            foreach (GameObject item in Zone.Objects)
            {
                if (item.Lot.Id == 3315)
                {
                    
                }
            }

            return false;
        }
    }
}