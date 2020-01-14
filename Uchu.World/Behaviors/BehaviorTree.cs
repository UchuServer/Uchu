using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.Behaviors
{
    public class BehaviorTree
    {
        private static Dictionary<BehaviorTemplateId, Type> _behaviors;

        public (int behaviorId, SkillCastType castType, int skillId)[] BehaviorIds { get; }

        public Dictionary<SkillCastType, List<BehaviorBase>> RootBehaviors { get; } =
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
            
            BehaviorIds = new (int behaviorId, SkillCastType castType, int skillId)[objectSkills.Length];

            for (var index = 0; index < objectSkills.Length; index++)
            {
                var objectSkill = objectSkills[index];
                
                var behavior = cdClient.SkillBehaviorTable.FirstOrDefault(b => b.SkillID == objectSkill.SkillID);
                
                if (behavior == default)
                {
                    Logger.Error($"Could not find behavior for skill: {objectSkill.SkillID}");
                
                    continue;
                }
                
                Logger.Information($"[{lot}] SKILL: {objectSkill.SkillID} -> {behavior.BehaviorID}");
                
                BehaviorIds[index] = (
                    behavior.BehaviorID.Value,
                    (SkillCastType) objectSkill.CastOnType.Value,
                    objectSkill.SkillID.Value
                );
            }
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
            
            BehaviorIds = new[] {(behavior.BehaviorID.Value, SkillCastType.Default, skillId)};
        }

        public async Task<BehaviorInfo[]> BuildAsync()
        {
            await using var ctx = new CdClientContext();
            
            foreach (var (id, castType, _) in BehaviorIds)
            {
                var root = BehaviorBase.Cache.FirstOrDefault(b => b.BehaviorId == id);

                if (root == default)
                {
                    var behavior = await ctx.BehaviorTemplateTable.FirstOrDefaultAsync(
                        t => t.BehaviorID == id
                    );

                    if (behavior?.TemplateID == null) continue;

                    var behaviorTypeId = (BehaviorTemplateId) behavior.TemplateID;
                    
                    if (!Behaviors.TryGetValue(behaviorTypeId, out var behaviorType))
                    {
                        Logger.Error($"No behavior type of \"{behaviorTypeId}\" found.");
                        
                        continue;
                    }

                    var instance = (BehaviorBase) Activator.CreateInstance(behaviorType);

                    instance.BehaviorId = id;

                    BehaviorBase.Cache.Add(instance);
                    
                    await instance.BuildAsync();
                }

                if (RootBehaviors.TryGetValue(castType, out var list))
                {
                    list.Add(root);
                }
                else
                {
                    RootBehaviors[castType] = new List<BehaviorBase> {root};
                }
            }

            return BehaviorIds.Select(b => new BehaviorInfo
            {
                SkillId = b.skillId,
                CastType = b.castType
            }).ToArray();
        }
        
        public async Task<ExecutionContext> ExecuteAsync(GameObject associate, BitReader reader, BitWriter writer, SkillCastType castType = SkillCastType.OnEquip, GameObject target = default, bool explicitTarget = false)
        {
            if (!explicitTarget)
                target = associate;

            var context = new ExecutionContext(associate, reader, writer);
            
            if (RootBehaviors.TryGetValue(SkillCastType.Default, out var defaultList))
            {
                foreach (var root in defaultList)
                {
                    context.Root = root;

                    var branchContext = new ExecutionBranchContext(target);
                    
                    await root.ExecuteAsync(context, branchContext);
                }
            }

            if (!RootBehaviors.TryGetValue(castType, out var list)) return context;
            
            foreach (var root in list)
            {
                context.Root = root;
                
                var branchContext = new ExecutionBranchContext(target);
                
                await root.ExecuteAsync(context, branchContext);
            }

            return context;
        }

        public async Task<ExecutionContext> UseAsync(GameObject associate, BitReader reader, BitWriter writer, GameObject target)
        {
            var context = new ExecutionContext(associate, reader, writer);

            if (!RootBehaviors.TryGetValue(SkillCastType.OnUse, out var list)) return context;
            
            foreach (var root in list)
            {
                context.Root = root;
                
                var branchContext = new ExecutionBranchContext(target);
                
                await root.ExecuteAsync(context, branchContext);
            }

            return context;
        }
        
        public async Task<ExecutionContext> MountAsync(GameObject associate)
        {
            var context = new ExecutionContext(associate, new BitReader(new MemoryStream()), new BitWriter(new MemoryStream()));

            if (!RootBehaviors.TryGetValue(SkillCastType.OnEquip, out var list)) return context;
            
            foreach (var root in list)
            {
                context.Root = root;
                
                var branchContext = new ExecutionBranchContext(associate);
                
                await root.ExecuteAsync(context, branchContext);
            }

            return context;
        }

        public async Task<ExecutionContext> DismantleAsync(GameObject associate)
        {
            var context = new ExecutionContext(associate, new BitReader(new MemoryStream()), new BitWriter(new MemoryStream()));

            if (!RootBehaviors.TryGetValue(SkillCastType.OnEquip, out var list)) return context;
            
            foreach (var root in list)
            {
                context.Root = root;
                
                var branchContext = new ExecutionBranchContext(associate);
                
                await root.DismantleAsync(context, branchContext);
            }

            return context;
        }

        public static async Task<BehaviorInfo[]> GetSkillsForItem(Item item)
        {
            var tree = new BehaviorTree(item.Lot);
            
            return await tree.BuildAsync();
        }
    }
}