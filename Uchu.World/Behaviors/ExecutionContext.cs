using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    using SyncDelegate = Func<BitReader, Task>;

    public class ExecutionContext
    {
        public GameObject Associate { get; }

        public BehaviorBase Root { get; set; }

        public BitReader Reader { get; set; }

        public BitWriter Writer { get; set; }

        public GameObject ExplicitTarget { get; set; }

        public List<BehaviorSyncEntry> BehaviorHandles { get; } = new List<BehaviorSyncEntry>();

        public ExecutionContext(GameObject associate, BitReader reader, BitWriter writer)
        {
            Associate = associate;
            Reader = reader;
            Writer = writer;
        }

        public async Task SyncAsync(uint handle, BitReader reader)
        {
            BehaviorSyncEntry entry;
            
            lock (BehaviorHandles)
            {
                entry = BehaviorHandles.FirstOrDefault(e => e.Handle == handle);

                if (entry == default)
                {
                    Logger.Error($"Invalid behavior sync id: {handle}!");
                    
                    return;
                }
                
                BehaviorHandles.Remove(entry);
            }
            
            await entry.Delegate(reader);
        }

        public void RegisterHandle(uint handle, SyncDelegate @delegate)
        {
            BehaviorHandles.Add(new BehaviorSyncEntry
            {
                Handle = handle,
                Delegate = @delegate
            });
        }

        public class BehaviorSyncEntry
        {
            public uint Handle { get; set; }

            public SyncDelegate Delegate { get; set; }
        }
    }
}