using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    using SyncDelegate = Func<BitReader, Task>;
    
    public class ExecutionContext
    {
        public readonly GameObject Associate;

        public BehaviorBase Root { get; set; }

        public BitReader Reader { get; set; }

        public uint SkillId { get; set; }
        
        public readonly Dictionary<uint, SyncDelegate> BehaviorHandles = new Dictionary<uint, SyncDelegate>();
        
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