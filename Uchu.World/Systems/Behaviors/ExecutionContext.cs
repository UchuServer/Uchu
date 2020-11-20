using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    using SyncDelegate = Action<BitReader>;
    
    public class ExecutionContext
    {
        public GameObject Associate { get; }

        public BehaviorBase Root { get; set; }

        /// <summary>
        /// Registered handles for sync skill messages
        /// </summary>
        private List<BehaviorSyncEntry> BehaviorHandles { get; } = new List<BehaviorSyncEntry>();

        public ExecutionContext(GameObject associate)
        {
            Associate = associate;
        }

        /// <summary>
        /// Syncs a the skill using a suncskill bitstream
        /// </summary>
        /// <param name="handle">The behavior handle to sync</param>
        /// <param name="reader">The sync skill bitstream</param>
        public void SyncAsync(uint handle, BitReader reader)
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
                
                // BehaviorHandles.Remove(entry);
            }

            entry.Delegate(reader);
        }

        public void RegisterHandle(uint handle, SyncDelegate @delegate)
        {
            lock (BehaviorHandles)
            {
                BehaviorHandles.Add(new BehaviorSyncEntry
                {
                    Handle = handle,
                    Delegate = @delegate
                });
                Logger.Debug($"Registered handle for sync id: {handle}");
            }
        }

        public class BehaviorSyncEntry
        {
            public uint Handle { get; set; }

            public SyncDelegate Delegate { get; set; }
        }
    }
}