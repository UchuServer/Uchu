using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    using SyncDelegate = Func<BitReader, Task>;
    
    public class ExecutionContext
    {
        public GameObject Associate { get; }

        public BehaviorBase Root { get; set; }

        public BitReader Reader { get; set; }

        public BitWriter Writer { get; set; }

        public GameObject ExplicitTarget { get; set; }

        private List<BehaviorSyncEntry> BehaviorHandles { get; } = new List<BehaviorSyncEntry>();

        /// <summary>
        /// Possible actions to execute, for example to send EchoSyncSkill messages
        /// </summary>
        private List<BehaviorActionEntry> BehaviorActions { get; } = new List<BehaviorActionEntry>();

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
        /// Registers a behavior action
        /// </summary>
        /// <param name="action">The action to register</param>
        /// <param name="parameters">The parameters to pass to the action when executed</param>
        public void RegisterAction(Action<BehaviorExecutionParameters> action, BehaviorExecutionParameters parameters)
        {
            lock (BehaviorActions)
            {
                BehaviorActions.Add(new BehaviorActionEntry()
                {
                    Action = action,
                    Parameters = parameters
                });
            }
        }

        /// <summary>
        /// Executes all the behavior actions in the background and removes them from the list
        /// </summary>
        public void ExecuteActions()
        {
            lock (BehaviorActions)
            {
                foreach (var action in BehaviorActions)
                {
                    Task.Run(() =>
                    {
                        action.Action(action.Parameters);
                    });
                    BehaviorActions.Remove(action);
                }
            }
        }

        public class BehaviorActionEntry
        {
            public BehaviorExecutionParameters Parameters { get; set; }
            public Action<BehaviorExecutionParameters> Action { get; set; }
        }

        public class BehaviorSyncEntry
        {
            public uint Handle { get; set; }

            public SyncDelegate Delegate { get; set; }
        }
    }
}