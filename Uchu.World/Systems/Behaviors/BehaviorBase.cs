using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Scripting.Native;

namespace Uchu.World.Systems.Behaviors
{
    public abstract class BehaviorBase<T> : BehaviorBase
        where T : BehaviorExecutionParameters, new()
    {
        protected virtual Task ExecuteStart(T executionParameters)
        {
            return Task.CompletedTask;
        }
        public override Task ExecuteStart(BehaviorExecutionParameters executionParameters)
        {
            return ExecuteStart((T) executionParameters);
        }

        protected virtual Task ExecuteSync(T executionParameters)
        {
            return Task.CompletedTask;
        }
        public override Task ExecuteSync(BehaviorExecutionParameters executionParameters)
        {
            return ExecuteSync((T) executionParameters);
        }

        protected virtual void DeserializeStart(T behaviorExecutionParameters)
        {
        }
        public override BehaviorExecutionParameters DeserializeStart(ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            var behaviorExecutionParameters = new T {Context = context, BranchContext = branchContext};
            DeserializeStart(behaviorExecutionParameters);
            return behaviorExecutionParameters;
        }

        protected virtual void DeserializeSync(T behaviorExecutionParameters)
        {
        }
        public override BehaviorExecutionParameters DeserializeSync(ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            var behaviorExecutionParameters = new T {Context = context, BranchContext = branchContext};
            DeserializeSync(behaviorExecutionParameters);
            return behaviorExecutionParameters;
        }
        
        protected virtual void SerializeStart(T behaviorExecutionParameters)
        {
        }
        public override BehaviorExecutionParameters SerializeStart(NpcExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            var behaviorExecutionParameters = new T {Context = context, BranchContext = branchContext};
            DeserializeStart(behaviorExecutionParameters);
            return behaviorExecutionParameters;
        }
        
        protected virtual void SerializeSync(T behaviorExecutionParameters)
        {
        }
        public override BehaviorExecutionParameters SerializeSync(NpcExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            var behaviorExecutionParameters = new T {Context = context, BranchContext = branchContext};
            DeserializeStart(behaviorExecutionParameters);
            return behaviorExecutionParameters;
        }

        protected virtual Task DismantleAsync(T executionParameters)
        {
            return Task.CompletedTask;
        }
        public override async Task DismantleAsync(BehaviorExecutionParameters executionParameters)
        {
            await DismantleAsync((T) executionParameters);
        }

        protected void RegisterHandle(uint handle, BehaviorExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.Context.RegisterHandle(handle, async reader =>
            {
                // TODO: Check if start is locked
                behaviorExecutionParameters.Context.Reader = reader;
                var syncBehaviorExecutionParameters = DeserializeSync(behaviorExecutionParameters.Context,
                    behaviorExecutionParameters.BranchContext);
                
                await ExecuteSync(syncBehaviorExecutionParameters);
            });
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

        public virtual Task ExecuteSync(BehaviorExecutionParameters executionParameters)
        {
            return Task.CompletedTask;
        }
        
        public virtual Task ExecuteStart(BehaviorExecutionParameters executionParameters)
        {
            return Task.CompletedTask;
        }

        public virtual Task DismantleAsync(BehaviorExecutionParameters executionParameters)
        {
            return Task.CompletedTask;
        }

        public virtual BehaviorExecutionParameters SerializeStart(NpcExecutionContext context, 
            ExecutionBranchContext branchContext)
        {
            return new BehaviorExecutionParameters(context, branchContext);
        }

        public virtual BehaviorExecutionParameters SerializeSync(NpcExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            return new BehaviorExecutionParameters(context, branchContext);
        }
        
        public virtual BehaviorExecutionParameters DeserializeStart(ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            return new BehaviorExecutionParameters(context, branchContext);
        }

        public virtual BehaviorExecutionParameters DeserializeSync(ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            return new BehaviorExecutionParameters(context, branchContext);
        }
    }
}