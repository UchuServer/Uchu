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
        //what is the bitstream serialization for this?
        //i'm not certain, but i would guess it's just 3 bools for each, i haven't tested this at all though
        public override async Task BuildAsync()
        {
            ShowCollectibles = await GetParameter<bool>("show_collectibles");
            ShowMiniBosses = await GetParameter<bool>("show_minibosses");
            ShowPetDigs = await GetParameter<bool>("show_pet_digs");
        }

        public override BehaviorExecutionParameters DeserializeStart(BitReader reader, ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            reader.ReadBit();
            reader.ReadBit();
            reader.ReadBit();
            return base.DeserializeStart(reader, context, branchContext);
        }

        public override BehaviorExecutionParameters SerializeStart(BitWriter writer, NpcExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            writer.WriteBit(ShowCollectibles);
            writer.WriteBit(ShowMiniBosses);
            writer.WriteBit(ShowPetDigs);
            return base.SerializeStart(writer, context, branchContext);
        }
    }
}