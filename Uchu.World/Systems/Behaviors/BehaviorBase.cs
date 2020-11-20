using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Scripting.Native;

namespace Uchu.World.Systems.Behaviors
{
    public abstract class BehaviorBase<T> : BehaviorBase
        where T : BehaviorExecutionParameters
    {
        /// <summary>
        /// Creates a new instance of this behavior execution parameter
        /// </summary>
        /// <param name="context">The context to pass to the parameters</param>
        /// <param name="branchContext">The branch context  to pass to the parameters</param>
        /// <returns>The new instance of <c>T</c></returns>
        private static T CreateInstance(ExecutionContext context, ExecutionBranchContext branchContext) =>
            (T) Activator.CreateInstance(typeof(T), context, branchContext);

        /// <summary>
        /// Executes the start skill of this behavior using the provided parameters
        /// </summary>
        /// <param name="parameters">The parameters to execute the start skill with</param>
        protected virtual void ExecuteStart(T parameters)
        {
        }
        
        /// <summary>
        /// Wrapper for the generic typed version of this method
        /// </summary>
        /// <param name="parameters">The parameters to execute the start skill with</param>
        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            ExecuteStart((T) parameters);
        }

        /// <summary>
        /// Executes the sync skill of this behavior using the provided parameters 
        /// </summary>
        /// <param name="parameters">The parameters to execute the sync skill with</param>
        protected virtual void ExecuteSync(T parameters)
        {
        }
        
        /// <summary>
        /// Wrapper for the generic typed version of this method
        /// </summary>
        /// <param name="parameters">The parameters to execute the sync skill with</param>
        public override void ExecuteSync(BehaviorExecutionParameters parameters)
        {
            ExecuteSync((T) parameters);
        }
        
        
        /// <summary>
        /// Undo the effects of an OnEquip using the provided parameters
        /// </summary>
        /// <param name="parameters">The parameters to execute the dismantle with</param>
        protected virtual void Dismantle(T parameters)
        {
        }
        
        /// <summary>
        /// Wrapper to call the generic typed version of this method
        /// </summary>
        /// <param name="parameters">The parameters to execute the dismantle with</param>
        public override void Dismantle(BehaviorExecutionParameters parameters)
        {
        }

        /// <summary>
        /// Serializes the passed behavior execution parameters
        /// </summary>
        /// <param name="behaviorExecutionParameters">The parameters to serialize</param>
        protected virtual void SerializeStart(BitWriter writer, T behaviorExecutionParameters)
        {
        }
        
        /// <summary>
        /// Wrapper to call the generic typed version of this method
        /// </summary>
        /// <param name="context">The global context to use</param>
        /// <param name="branchContext">The branch context to use</param>
        /// <returns><c>BehaviorExecutionParameters</c> gained by serializing the start skill</returns>
        public override BehaviorExecutionParameters SerializeStart(BitWriter writer, NpcExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            var behaviorExecutionParameters = CreateInstance(context, branchContext);
            SerializeStart(writer, behaviorExecutionParameters);
            return behaviorExecutionParameters;
        }
        
        /// <summary>
        /// Deserializes behavior execution parameters using a bitstream
        /// </summary>
        /// <param name="parameters">The parameters to deserialize using its bitstream</param>
        protected virtual void SerializeSync(BitWriter writer, T parameters)
        {
        }
        
        /// <summary>
        /// Deserializes a sync skill bitstream using the provided context and branch context
        /// </summary>
        /// <param name="context">The global context to use</param>
        /// <param name="branchContext">The branch context to use</param>
        /// <returns><c>BehaviorExecutionParameters</c> gained by deserializing the sync skill</returns>
        public override void SerializeSync(BitWriter writer, BehaviorExecutionParameters parameters)
        {
            SerializeSync(writer, (T)parameters);
        }
        
        /// <summary>
        /// Deserializes the provided parameters using a bitstream
        /// </summary>
        /// <param name="parameters">The parameters to deserialize</param>
        protected virtual void DeserializeStart(BitReader reader, T parameters)
        {
        }
        
        /// <summary>
        /// Deserializes a start skill bitstream using the provided context and branch context
        /// </summary>
        /// <param name="context">The global context to use</param>
        /// <param name="branchContext">The branch context to use</param>
        /// <returns><c>BehaviorExecutionParameters</c> gained by deserializing the start skill</returns>
        public override BehaviorExecutionParameters DeserializeStart(BitReader reader, ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            var behaviorExecutionParameters = CreateInstance(context, branchContext);
            DeserializeStart(reader, behaviorExecutionParameters);
            return behaviorExecutionParameters;
        }

        /// <summary>
        /// Deserializes behavior execution parameters using a bitstream
        /// </summary>
        /// <param name="parameters">The parameters to deserialize using its bitstream</param>
        protected virtual void DeserializeSync(BitReader reader, T parameters)
        {
        }
        
        /// <summary>
        /// Deserializes a sync skill bitstream using the provided context and branch context
        /// </summary>
        /// <param name="context">The global context to use</param>
        /// <param name="branchContext">The branch context to use</param>
        /// <returns><c>BehaviorExecutionParameters</c> gained by deserializing the sync skill</returns>
        public override BehaviorExecutionParameters DeserializeSync(BitReader reader, ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            var behaviorExecutionParameters = CreateInstance(context, branchContext);
            DeserializeSync(reader, behaviorExecutionParameters);
            return behaviorExecutionParameters;
        }

        public async Task PlayFxAsync(string type, GameObject target, int time)
        {
            await using var ctx = new CdClientContext();

            var fx = await ctx.BehaviorEffectTable.FirstOrDefaultAsync(
                e => e.EffectType == type && e.EffectID == EffectId
            );
            
            if (fx == default) return;

            target.PlayFX(fx.EffectName, fx.EffectType, EffectId);

            var _ = Task.Run(async () =>
            {
                await Task.Delay(time);
                target.StopFX(fx.EffectName);
            });
        }
    }

    public abstract class BehaviorBase
    {
        public int BehaviorId { get; set; }
        public abstract BehaviorTemplateId Id { get; }
        protected int EffectId { get; set; }
        protected string EffectHandler { get; set; }
        
        public static readonly List<BehaviorBase> Cache = new List<BehaviorBase>();
        
        public virtual Task BuildAsync()
        {
            return Task.CompletedTask;
        }

        protected async Task<BehaviorBase> GetBehavior(string name)
        {
            var action = await GetParameter(name);
            if (action?.Value == null || action.Value.Value.Equals(0))
                return new EmptyBehavior();
            return await BuildBranch((int) action.Value);
        }

        protected static async Task<BehaviorBase> GetBehavior(uint id)
        {
            if (id == default)
                return new EmptyBehavior();
            return await BuildBranch((int) id);
        }

        private static async Task<BehaviorBase> BuildBranch(int behaviorId)
        {
            var cachedBehavior = Cache.ToArray().FirstOrDefault(c => c.BehaviorId == behaviorId);
            if (cachedBehavior != default) return cachedBehavior;
            
            await using var ctx = new CdClientContext();

            var behavior = await ctx.BehaviorTemplateTable.FirstOrDefaultAsync(
                t => t.BehaviorID == behaviorId
            );
            
            if (behavior?.TemplateID == null)
                return new EmptyBehavior();
            
            var behaviorTypeId = (BehaviorTemplateId) behavior.TemplateID;
            
            if (!BehaviorTree.Behaviors.TryGetValue(behaviorTypeId, out var behaviorType))
            {
                Logger.Error($"No behavior type of \"{behaviorTypeId}\" found.");
                return new EmptyBehavior();
            }

            var instance = (BehaviorBase) Activator.CreateInstance(behaviorType);
            
            instance.BehaviorId = behaviorId;
            instance.EffectId = behavior.EffectID ?? 0;
            instance.EffectHandler = behavior.EffectHandle;
            
            Cache.Add(instance);

            await instance.BuildAsync();

            return instance;
        }
        
        protected async Task<BehaviorParameter> GetParameter(string name)
        {
            await using var cdClient = new CdClientContext();
            return await cdClient.BehaviorParameterTable.FirstOrDefaultAsync(p =>
                p.BehaviorID == BehaviorId && p.ParameterID == name
            );
        }

        protected async Task<TS> GetParameter<TS>(string name) where TS : struct
        {
            var param = await GetParameter(name);

            if (param == default)
                return default;
            
            return param.Value.HasValue 
                ? (TS) Convert.ChangeType(param.Value.Value, typeof(TS)) 
                : default;
        }

        protected BehaviorParameter[] GetParameters()
        {
            using var cdClient = new CdClientContext();
            return cdClient.BehaviorParameterTable.Where(p =>
                p.BehaviorID == BehaviorId
            ).ToArray();
        }

        public async Task<BehaviorTemplate> GetTemplate()
        {
            await using var cdClient = new CdClientContext();
            return await cdClient.BehaviorTemplateTable.FirstOrDefaultAsync(p =>
                p.BehaviorID == BehaviorId
            );
        }
        
        /// <summary>
        /// Undo the effects of an OnEquip using the provided parameters
        /// </summary>
        /// <param name="parameters">The parameters to execute the dismantle with</param>
        public virtual void Dismantle(BehaviorExecutionParameters parameters)
        {
        }

        /// <summary>
        /// Executes the sync skill of this behavior using the provided parameters 
        /// </summary>
        /// <param name="parameters">The parameters to execute the sync skill with</param>
        public virtual void ExecuteSync(BehaviorExecutionParameters parameters)
        {
        }
        
        /// <summary>
        /// Executes the start skill of this behavior using the provided parameters
        /// </summary>
        /// <param name="parameters">The parameters to execute the start skill with</param>
        public virtual void ExecuteStart(BehaviorExecutionParameters parameters)
        {
        }

        /// <summary>
        /// Serializes a start skill bitstream using the provided context and branch context
        /// </summary>
        /// <param name="context">The global context to use</param>
        /// <param name="branchContext">The branch context to use</param>
        /// <returns><c>BehaviorExecutionParameters</c> gained by serializing the start skill</returns>
        public virtual BehaviorExecutionParameters SerializeStart(BitWriter writer, NpcExecutionContext context, 
            ExecutionBranchContext branchContext) => new BehaviorExecutionParameters(context, branchContext);

        /// <summary>
        /// Creates the behavior execution parameters for a certain behavior given a context and a branch context
        /// </summary>
        /// <param name="context">The global context to use</param>
        /// <param name="branchContext">The branch specific context to use</param>
        public virtual void SerializeSync(BitWriter writer, BehaviorExecutionParameters parameters)
        {
        }
        
        /// <summary>
        /// Deserializes a start skill bitstream using the provided context and branch context
        /// </summary>
        /// <param name="context">The global context to use</param>
        /// <param name="branchContext">The branch context to use</param>
        /// <returns><c>BehaviorExecutionParameters</c> gained by deserializing the start skill</returns>
        public virtual BehaviorExecutionParameters DeserializeStart(BitReader reader, ExecutionContext context,
            ExecutionBranchContext branchContext) => new BehaviorExecutionParameters(context, branchContext);

        /// <summary>
        /// Deserializes a sync skill bitstream using the provided context and branch context
        /// </summary>
        /// <param name="context">The global context to use</param>
        /// <param name="branchContext">The branch context to use</param>
        /// <returns><c>BehaviorExecutionParameters</c> gained by deserializing the sync skill</returns>
        public virtual BehaviorExecutionParameters DeserializeSync(BitReader reader, ExecutionContext context,
            ExecutionBranchContext branchContext) => new BehaviorExecutionParameters(context, branchContext);
    }
}