using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    using SyncDelegate = Func<BitReader, BitWriter, Task>;

    public class ExecutionContext
    {
        public GameObject Associate { get; }

        public BehaviorBase Root { get; set; }

        public BitReader Reader { get; set; }
        
        public BitWriter Writer { get; set; }
        
        public uint SkillId { get; set; }
        
        public Dictionary<uint, SyncDelegate> BehaviorHandles { get; } = new Dictionary<uint, SyncDelegate>();
        
        public ExecutionContext(GameObject associate, BitReader reader, BitWriter writer)
        {
            Associate = associate;
            Reader = reader;
            Writer = writer;
        }

        public async Task SyncAsync(uint handle, BitReader reader, BitWriter writer)
        {
            if (BehaviorHandles.TryGetValue(handle, out var method))
            {
                await method(reader, writer);

                BehaviorHandles.Remove(handle);
            }
        }
    }
}