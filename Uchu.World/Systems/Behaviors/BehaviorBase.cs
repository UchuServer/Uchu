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
    public abstract class BehaviorBase
    {
        public static readonly List<BehaviorBase> Cache = new List<BehaviorBase>();
        
        public int BehaviorId { get; set; }

        public abstract BehaviorTemplateId Id { get; }

        public abstract Task BuildAsync();

        public int EffectId { get; set; }
        
        public string EffectHandler { get; set; }

        public virtual Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            return Task.CompletedTask;
        }

        public virtual Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            return Task.CompletedTask;
        }

        public virtual Task SyncAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            return Task.CompletedTask;
        }

        public virtual Task DismantleAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            return Task.CompletedTask;
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

        public static async Task<BehaviorBase> BuildBranch(int behaviorId)
        {
            var cachedBehavior = Cache.ToArray().FirstOrDefault(c => c.BehaviorId == behaviorId);

            if (cachedBehavior != default) return cachedBehavior;
            
            await using var ctx = new CdClientContext();

            var behavior = await ctx.BehaviorTemplateTable.FirstOrDefaultAsync(
                t => t.BehaviorID == behaviorId
            );
            
            if (behavior?.TemplateID == null) return new EmptyBehavior();
            
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

        protected void RegisterHandle(uint handle, ExecutionContext context, ExecutionBranchContext branchContext)
        {
            context.RegisterHandle(handle, async reader =>
            {
                var newBranchContext = new ExecutionBranchContext(branchContext.Target)
                {
                    Duration = branchContext.Duration
                };

                context.Reader = reader;

                await SyncAsync(context, newBranchContext);
            });
        }

        protected async Task<BehaviorParameter> GetParameter(string name)
        {
            await using var cdClient = new CdClientContext();
            return await cdClient.BehaviorParameterTable.FirstOrDefaultAsync(p =>
                p.BehaviorID == BehaviorId && p.ParameterID == name
            );
        }

        protected async Task<T> GetParameter<T>(string name) where T : struct
        {
            var param = await GetParameter(name);

            if (param == default) return default;
            
            return param.Value.HasValue ? (T) Convert.ChangeType(param.Value.Value, typeof(T)) : default;
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

        protected async Task<BehaviorBase> GetBehavior(string name)
        {
            var action = await GetParameter(name);

            if (action?.Value == null || action.Value.Value.Equals(0)) return new EmptyBehavior();

            return await BuildBranch((int) action.Value);
        }

        protected async Task<BehaviorBase> GetBehavior(uint id)
        {
            if (id == default) return new EmptyBehavior();
            
            return await BuildBranch((int) id);
        }
    }
}