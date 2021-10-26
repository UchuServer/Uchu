using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class ModuleAssemblyComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.ModuleAssemblyComponent;

        private string GetAssemblyData() {
            return GameObject.Settings.ToString();
        }

        public override void Construct(BitWriter writer)
        {
            if (writer.Flag(true))
            {
                if (writer.Flag(false)) 
                    writer.Write((long)0); // Subkey

                writer.WriteBit(false); // Use optional Parts?

                var assemblyData = GetAssemblyData();
                writer.Write((ushort) assemblyData.Length);
                writer.WriteString(assemblyData, assemblyData.Length, true);
            }
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}