using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Uchu.Core.Client;
using Uchu.World.Scripting.Native;

namespace Uchu.World.Systems.Behaviors
{
    public class BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            Context = context;
            BranchContext = new ExecutionBranchContext
            {
                Duration = branchContext.Duration,
                Target = branchContext.Target
            };
        }
        
        /// <summary>
        /// The root context for the entirety of the behavior execution 
        /// </summary>
        public ExecutionContext Context { get; set; }
        
        /// <summary>
        /// If the behavior branched, extra information for execution in the branch
        /// </summary>
        public ExecutionBranchContext BranchContext { get; set; }

        /// <summary>
        /// Returns the context as a NpcContext, can cause a NullReference if the context can't be casted.
        /// </summary>
        public NpcExecutionContext NpcContext
        {
            get => (NpcExecutionContext) Context;
            set => Context = value;
        }
        
        /// <summary>
        /// Schedules a task to be executed by the game loop at a later time
        /// </summary>
        /// <remarks>
        /// Primarily used to delay certain behaviors.
        /// </remarks>
        /// <param name="delay">The delay in milliseconds to execute the action, if set le 0 this will execute on the next tick</param>
        /// <param name="delegate">The action to execute</param>
        public void Schedule(Action @delegate, float delay = 0)
        {
            Context.Associate.Zone.Schedule(@delegate, delay);
        }

        /// <summary>
        /// Provides net favor for a certain task.
        /// </summary>
        /// <remarks>
        /// Executes if the branch context target has a ping of 0, schedules if the branch context target has a ping > 0.
        /// </remarks>
        /// <param name="delegate">The action to execute or schedule</param>
        public void NetFavor(Action @delegate)
        {
            var ping = (BranchContext.Target as Player)?.Ping ?? 0;
            if (ping == 0)
            {
                @delegate();
            }
            else
            {
                Schedule(@delegate, ping);
            }
        }

        /// <summary>
        /// Registers a handle for syncing the behavior this context represents
        /// </summary>
        /// <param name="handle">The handle to register for syncing</param>
        /// <param name="deserializeSync">Function to deserialize the sync skill packet</param>
        /// <param name="executeSync">Function to execute the sync skill packet</param>
        public void RegisterHandle<T>(uint handle, Action<T> deserializeSync, Action<T> executeSync) 
            where T : BehaviorExecutionParameters
        {
            Context.RegisterHandle(handle, reader =>
            {
                // Only one sync may be deserialized at the same time
                Context.Lock.WaitOne();
                
                var syncParameters = (T)Activator.CreateInstance(typeof(T), Context, BranchContext);
                try
                {
                    Context.Reader = reader;
                    deserializeSync(syncParameters);
                }
                finally
                {
                    Context.Lock.ReleaseMutex();
                }
                
                executeSync(syncParameters);
            });
        }

        /// <summary>
        /// Plays an effect
        /// </summary>
        /// <remarks>
        /// Schedules a task in the game loop to stop the Fx from playing.
        /// Needs to look up the effect in the behavior effect table.
        /// </remarks>
        /// <param name="type">The effect type</param>
        /// <param name="effectId">The effect to execute</param>
        /// <param name="target">The effect target</param>
        /// <param name="time">How long to run the effect in milliseconds</param>
        public async void PlayFX(string type, int effectId, int time = 1000, GameObject target = default)
        {
            target ??= BranchContext.Target;
            
            await using var ctx = new CdClientContext();

            var fx = await ctx.BehaviorEffectTable.FirstOrDefaultAsync(
                e => e.EffectType == type && e.EffectID == effectId
            );
            
            if (fx == default)
                return;

            // Play the effect and schedule it's completion
            target.PlayFX(fx.EffectName, fx.EffectType, effectId);
            Schedule(() =>
            {
                target.StopFX(fx.EffectName);
            }, time);
        }
    }
}