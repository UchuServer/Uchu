using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;

namespace Uchu.World.Behaviors
{
    public class BehaviorTree
    {
        private static Dictionary<BehaviorTemplateId, Type> _behaviors;
        
        public readonly (int, SkillCastType)[] BehaviorIds;

        public readonly Dictionary<SkillCastType, List<BehaviorBase>> RootBehaviors =
            new Dictionary<SkillCastType, List<BehaviorBase>>();

        public static Dictionary<BehaviorTemplateId, Type> Behaviors
        {
            get
            {
                if (_behaviors != default) return _behaviors;

                _behaviors = new Dictionary<BehaviorTemplateId, Type>();

                var behaviors = typeof(BehaviorBase).Assembly.GetTypes().Where(
                    t => t.BaseType == typeof(BehaviorBase) && t != typeof(BehaviorBase)
                ).ToArray();

                foreach (var behavior in behaviors)
                {
                    _behaviors.Add(((BehaviorBase) Activator.CreateInstance(behavior)).Id, behavior);
                }

                return _behaviors;
            }
        }
        
        public BehaviorTree(Lot lot)
        {
            using var cdClient = new CdClientContext();

            var objectSkills = cdClient.ObjectSkillsTable.Where(i =>
                i.ObjectTemplate == lot
            ).ToArray();

            var skillBehaviors = cdClient.SkillBehaviorTable.Where(
                b => objectSkills.Any(s => s.SkillID == b.SkillID)
            ).ToArray();

            BehaviorIds = skillBehaviors.Where(
                s => s.BehaviorID != null && s.CastTypeDesc != null
            ).Select(
                s => (s.BehaviorID.Value, (SkillCastType) s.CastTypeDesc.Value)
            ).ToArray();
        }
        
        public BehaviorTree(int skillId)
        {
            using var cdClient = new CdClientContext();

            var behavior = cdClient.SkillBehaviorTable.FirstOrDefault(b => b.SkillID == skillId);

            if (behavior == default)
            {
                Logger.Error($"Could not find behavior for skill: {skillId}");
                
                return;
            }

            if (behavior.BehaviorID == default)
            {
                Logger.Error($"{skillId} has an invalid behavior id");
                
                return;
            }
            
            BehaviorIds = new[] {(behavior.BehaviorID.Value, SkillCastType.Default)};
        }

        public async Task BuildAsync()
        {
            await using var ctx = new CdClientContext();
            
            foreach (var (id, castType) in BehaviorIds)
            {
                var behavior = await ctx.BehaviorTemplateTable.FirstOrDefaultAsync(
                    t => t.BehaviorID == id
                );

                if (behavior?.TemplateID == null) continue;

                var behaviorType = Behaviors[(BehaviorTemplateId) behavior.TemplateID];

                var instance = (BehaviorBase) Activator.CreateInstance(behaviorType);

                instance.BehaviorId = id;
                instance.ParentNode = new EmptyBehavior();

                if (RootBehaviors.TryGetValue(castType, out var list))
                {
                    list.Add(instance);
                }
                else
                {
                    RootBehaviors[castType] = new List<BehaviorBase> {instance};
                }
                
                await instance.BuildAsync();
            }
        }

        public async Task<ExecutionContext> ExecuteAsync(GameObject associate, BitReader reader, SkillCastType castType = SkillCastType.OnEquip)
        {
            var context = new ExecutionContext(associate, reader);
            
            if (RootBehaviors.TryGetValue(SkillCastType.Default, out var defaultList))
            {
                foreach (var root in defaultList)
                {
                    context.Root = root;
                    
                    (associate as Player)?.SendChatMessage($"EXEC: [{root.GetType().Name}] {root.BehaviorId}");
                    
                    await root.ExecuteAsync(context, new ExecutionBranchContext(default));
                }
            }

            if (!RootBehaviors.TryGetValue(castType, out var list)) return context;
            
            foreach (var root in list)
            {
                context.Root = root;
                
                (associate as Player)?.SendChatMessage($"EXEC: [{root.GetType().Name}] {root.BehaviorId}");
                
                await root.ExecuteAsync(context, new ExecutionBranchContext(default));
            }

            return context;
        }
    }
}