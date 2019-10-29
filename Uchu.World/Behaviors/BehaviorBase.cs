using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core.CdClient;

namespace Uchu.World.Behaviors
{
    public abstract class BehaviorBase
    {
        public static readonly List<BehaviorBase> Cache = new List<BehaviorBase>();
        
        public int BehaviorId { get; set; }

        public abstract BehaviorTemplateId Id { get; }

        public abstract Task BuildAsync();

        public virtual Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            return Task.CompletedTask;
        }

        public virtual Task SyncAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            return Task.CompletedTask;
        }

        public static async Task<BehaviorBase> BuildBranch(int behaviorId)
        {
            var cachedBehavior = Cache.FirstOrDefault(c => c.BehaviorId == behaviorId);

            if (cachedBehavior != default) return cachedBehavior;
            
            await using var ctx = new CdClientContext();

            var behavior = await ctx.BehaviorTemplateTable.FirstOrDefaultAsync(
                t => t.BehaviorID == behaviorId
            );
            
            if (behavior?.TemplateID == null) return new EmptyBehavior();
            
            var id = (BehaviorTemplateId) behavior.TemplateID;

            var instance = (BehaviorBase) Activator.CreateInstance(BehaviorTree.Behaviors[id]);
            
            instance.BehaviorId = behaviorId;
            
            Cache.Add(instance);

            await instance.BuildAsync();

            return instance;
        }

        public void RegisterHandle(uint handle, ExecutionContext context, ExecutionBranchContext branchContext)
        {
            context.BehaviorHandles[handle] = async reader =>
            {
                context.Reader = reader;
                
                await SyncAsync(context, branchContext);
            };
        }

        public async Task<BehaviorParameter> GetParameter(string name)
        {
            await using var cdClient = new CdClientContext();
            return await cdClient.BehaviorParameterTable.FirstOrDefaultAsync(p =>
                p.BehaviorID == BehaviorId && p.ParameterID == name
            );
        }
        
        public async Task<T> GetParameter<T>(string name) where T : struct
        {
            var param = await GetParameter(name);

            if (param == default) return default;
            
            return param.Value.HasValue ? (T) Convert.ChangeType(param.Value.Value, typeof(T)) : default;
        }

        public BehaviorParameter[] GetParameters()
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

        public async Task<BehaviorBase> GetBehavior(string name)
        {
            var action = await GetParameter(name);

            if (action?.Value == null) return new EmptyBehavior();

            return await BuildBranch((int) action.Value);
        }
        
        public async Task<BehaviorBase> GetBehavior(uint id)
        {
            if (id == default) return new EmptyBehavior();
            
            return await BuildBranch((int) id);
        }
    }
}