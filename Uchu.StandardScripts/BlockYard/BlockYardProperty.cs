using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.StandardScripts;

namespace Uchu.StandardScripts.BlockYard
{
    [ZoneSpecific(1150)]
    public class BlockYardProperty : Property.BaseProperty
    {
        public override Task LoadAsync()
        {


            return Task.CompletedTask;
        }
    }
}