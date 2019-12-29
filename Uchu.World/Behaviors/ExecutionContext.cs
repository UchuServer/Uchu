using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    using SyncDelegate = Func<BitReader, Task>;
    
    using DismantleDelegate = Func<Task>;

    public class ExecutionContext
    {
        public GameObject Associate { get; }

        public BehaviorBase Root { get; set; }

        public BitReader Reader { get; set; }

        public uint SkillId { get; set; }
        
        public Dictionary<uint, SyncDelegate> BehaviorHandles { get; } = new Dictionary<uint, SyncDelegate>();
        
        public ExecutionContext(GameObject associate, BitReader reader)
        {
            Associate = associate;
            Reader = reader;
        }

        public async Task SyncAsync(uint handle, BitReader reader)
        {
            if (BehaviorHandles.TryGetValue(handle, out var method))
            {
                await method(reader);

                BehaviorHandles.Remove(handle);
            }
        }
    }
}