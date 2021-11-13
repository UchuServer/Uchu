
namespace Uchu.World
{
    public class ModuleAssemblyComponent : StructReplicaComponent<ModuleAssemblySerialization>
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

        public override ModuleAssemblySerialization GetConstructPacket()
        {
            var packet = base.GetConstructPacket();
            packet.ModuleAssemblyInfo = new ModuleAssemblyInfo
            {
                Assembly = GameObject.InvalidObject, // this.GameObject ? subkey ?
                Blob = _parts,
                UseOptionalParts = false,
            };
            packet.Flag = true;
            return packet;
        }

        public override ModuleAssemblySerialization GetSerializePacket()
        {
            var packet = this.GetConstructPacket();
            packet.Flag = false;
            return packet;
        }

    }
}
