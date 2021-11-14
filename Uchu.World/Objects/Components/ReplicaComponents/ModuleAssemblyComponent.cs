
namespace Uchu.World
{
    public class ModuleAssemblyComponent : StructReplicaComponent<ModuleAssemblyConstruction, ModuleAssemblySerialization>
    {
        public override ComponentId Id => ComponentId.ModuleAssemblyComponent;

        private string _parts;

        public void SetAssembly(string parts)
        {
            this._parts = parts;
        }

        public string GetAssembly()
        {
            return this._parts;
        }

        public override ModuleAssemblyConstruction GetConstructPacket()
        {
            var packet = base.GetConstructPacket();
            packet.ModuleAssemblyInfo = new ModuleAssemblyInfo
            {
                Assembly = GameObject.InvalidObject, // subkey
                Blob = _parts,
                UseOptionalParts = false,
            };
            packet.Flag = true;
            return packet;
        }

        public override ModuleAssemblySerialization GetSerializePacket()
        {
            var packet = base.GetSerializePacket();
            return packet;
        }
    }
}
