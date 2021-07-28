using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    using SyncDelegate = Action<BitReader>;
    
    /// <summary>
    /// Base context of executing a behavior, contains the caster and the behavior to execute
    /// </summary>
    public class ExecutionContext
    {
        /// <summary>
        /// Lock used for sync skill deserialization
        /// </summary>
        public Mutex Lock { get; } = new Mutex();
        
        /// <summary>
        /// The caster of the behavior
        /// </summary>
        public GameObject Associate { get; }

        /// <summary>
        /// The behavior associated with this context
        /// </summary>
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
        /// Syncs a the skill using a syncskill bitstream
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
            }

            entry.Delegate(reader);
        }

        /// <summary>
        /// Registers a behavior handle to be synced later
        /// </summary>
        /// <param name="handle">The behavior handle to use as sync index</param>
        /// <param name="delegate">The function to execute on sync</param>
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

        /// <summary>
        /// Entry in the behavior sync list
        /// </summary>
        private class BehaviorSyncEntry
        {
            /// <summary>
            /// Behavior index to match on sync
            /// </summary>
            public uint Handle { get; set; }

            /// <summary>
            /// Function to execute on sync
            /// </summary>
            public SyncDelegate Delegate { get; set; }
        }
    }
}