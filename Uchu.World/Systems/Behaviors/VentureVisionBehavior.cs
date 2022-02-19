using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class VentureVisionBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.VentureVision;
        private bool ShowCollectibles { get; set; }
        private bool ShowMiniBosses { get; set; }
        private bool ShowPetDigs { get; set; }
        //TODO: find correct serialization
        public override async Task BuildAsync()
        {
            ShowCollectibles = await GetParameter<bool>("show_collectibles");
            ShowMiniBosses = await GetParameter<bool>("show_minibosses");
            ShowPetDigs = await GetParameter<bool>("show_pet_digs");
        }
    }
}