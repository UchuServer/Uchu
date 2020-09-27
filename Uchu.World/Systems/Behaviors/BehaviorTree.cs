using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Scripting.Runtime;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.Systems.Behaviors
{
    public class BehaviorExecution
    {
        public BehaviorBase BehaviorBase { get; set; }
        public BehaviorExecutionParameters BehaviorExecutionParameters { get; set; }
    }
    
    public class BehaviorTree
    {
        /// <summary>
        /// Creates a behavior tree
        /// </summary>
        private BehaviorTree()
        {
        }
        
        /// <summary>
        /// Contains all the abstract information about the database behaviors in this tree
        /// </summary>
        public BehaviorInfo[] BehaviorIds { get; private set; }

        /// <summary>
        /// All the implemented behaviors in this tree, stored by ID
        /// </summary>
        public Dictionary<int, BehaviorBase> SkillRoots { get; set; } = new Dictionary<int, BehaviorBase>();

        /// <summary>
        /// All the behaviors in this tree, grouped by SkillCastType
        /// </summary>
        private Dictionary<SkillCastType, List<BehaviorExecution>> RootBehaviors { get; } =
            new Dictionary<SkillCastType, List<BehaviorExecution>>();
        
        private Dictionary<BehaviorBase, BehaviorExecutionParameters> TreeExecutionParameters;

        private static Dictionary<BehaviorTemplateId, Type> _behaviors;

        private SkillCastType CastType = default;
        
        /// <summary>
        /// Map of all implemented behaviors, ordered by behavior template id
        /// </summary>
        public static Dictionary<BehaviorTemplateId, Type> Behaviors
        {
            get
            {
                if (_behaviors != default) return _behaviors;
                _behaviors = new Dictionary<BehaviorTemplateId, Type>();

                // Get all implemented behaviors for the assembly
                var behaviors = typeof(BehaviorBase).Assembly.GetTypes().Where(
                    t => t.BaseType == typeof(BehaviorBase) && t != typeof(BehaviorBase)
                ).ToArray();

                // Store all found behaviors by their skill ID
                foreach (var behavior in behaviors)
                {
                    var behaviorInstance = Activator.CreateInstance(behavior);
                    if (behaviorInstance == default)
                    {
                        Logger.Error($"Could not create instance of behavior of type {behavior}.");
                        continue;
                    }
                    _behaviors.Add(((BehaviorBase) behaviorInstance).Id, behavior);
                }

                return _behaviors;
            }
        }

        public static async Task<BehaviorTree> FromLotAsync(Lot lot)
        {
            var tree = new BehaviorTree();

            await using var cdClient = new CdClientContext();

            var objectSkills = cdClient.ObjectSkillsTable.Where(i =>
                i.ObjectTemplate == lot
            ).ToArray();

            tree.BehaviorIds = new BehaviorInfo[objectSkills.Length];

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

                tree.BehaviorIds[index] = new BehaviorInfo
                {
                    BaseBehavior = behavior.BehaviorID ?? 0,
                    CastType = objectSkill.CastOnType.HasValue ? (SkillCastType) objectSkill.CastOnType : SkillCastType.OnUse,
                    SkillId = objectSkill.SkillID ?? 0,
                    ImaginationCost = behavior.Imaginationcost ?? 0
                };
            }
            
            await tree.BuildAsync();

            return tree;
        }

        /// <summary>
        /// Takes a skill ID and creates a simple skill tree with database information about that behavior skill
        /// </summary>
        /// <param name="skillId">The skill ID to use</param>
        /// <returns>A behavior tree that contains database information about the skill belonging to the skill ID</returns>
        public static async Task<BehaviorTree> FromSkillAsync(int skillId)
        {
            var tree = new BehaviorTree();
            
            // Try to find the base behavior linked to this skill
            var behavior = await BaseBehaviorForSkill(skillId);
            if (behavior?.BehaviorID == default)
                return tree;

            tree.BehaviorIds = new[]
            {
                new BehaviorInfo
                {
                    SkillId = skillId,
                    CastType =  SkillCastType.Default,
                    BaseBehavior = behavior.BehaviorID.Value,
                    ImaginationCost = behavior.Imaginationcost ?? 0
                }
            };

            await tree.BuildAsync();
            return tree;
        }

        /// <summary>
        /// Finds the root behavior associated with a given skill by looking in the SkillBehavior table and returns
        /// database information regarding that root behavior.
        /// </summary>
        /// <param name="skillId">The skill ID to find a behavior for</param>
        /// <returns>The skill behavior if it existed, <c>default</c> otherwise</returns>
        private static async Task<SkillBehavior> BaseBehaviorForSkill(int skillId)
        {
            await using var clientContext = new CdClientContext();
            var skillBehavior = clientContext.SkillBehaviorTable.FirstOrDefault(b => b.SkillID == skillId);
            
            if (skillBehavior == default)
            {
                Logger.Error($"Could not find behavior for skill: {skillId}.");
            }
            else if (skillBehavior.BehaviorID == default)
            {
                Logger.Error($"{skillId} has an invalid behavior id.");
            }
            
            return skillBehavior;
        }

        /// <summary>
        /// Builds the skill tree by taking all database information about the behaviors in the tree and turning them
        /// into implemented classes for each respective database behavior.
        /// </summary>
        private async Task BuildAsync()
        {
            await using var ctx = new CdClientContext();

            // Build the base behavior for each requested skill
            foreach (var skill in BehaviorIds)
            {
                var root = BehaviorBase.Cache.ToArray().FirstOrDefault(
                    b => b.BehaviorId == skill.BaseBehavior);

                // If the behavior can't be found in the cache, build it from scratch using its template
                if (root == default)
                {
                    root = await BehaviorFromInfo(ctx, skill);
                    if (root == null)
                    {
                        continue;
                    }
                }

                // For each skill, store the fresh behavior root
                SkillRoots[skill.SkillId] = root;

                var executionPreparation = new BehaviorExecution()
                {
                    BehaviorBase = root
                };
                
                // Also store each behavior by CastType
                if (RootBehaviors.TryGetValue(skill.CastType, out var list))
                {
                    list.Add(executionPreparation);
                }
                else
                {
                    RootBehaviors[skill.CastType] = new List<BehaviorExecution>
                        { executionPreparation };
                }
            }
        }

        /// <summary>
        /// Finds and builds a behavior implementation using behavior database information.
        /// </summary>
        /// <remarks>
        /// Also adds this behavior implementation to the cache
        /// </remarks>
        /// <param name="context">Reusable context to query from</param>
        /// <param name="info">The behavior info to get the skill from</param>
        /// <returns>The instantiated behavior base if succeeded, <c>null</c> otherwise</returns>
        private static async Task<BehaviorBase> BehaviorFromInfo(CdClientContext context, BehaviorInfo info)
        {
            var behavior = await context.BehaviorTemplateTable.FirstOrDefaultAsync(
                t => t.BehaviorID == info.BaseBehavior
            );

            if (behavior?.TemplateID == null)
                return null;
            
            var behaviorTypeId = (BehaviorTemplateId) behavior.TemplateID;

            if (!Behaviors.TryGetValue(behaviorTypeId, out var behaviorType))
            {
                Logger.Error($"No behavior type of \"{behaviorTypeId}\" found.");
                return null;
            }

            var instance = (BehaviorBase) Activator.CreateInstance(behaviorType);
            if (instance == default)
            {
                Logger.Error($"Could not create behaviour of type {behaviorType}.");
                return null;
            }
            
            instance.BehaviorId = info.BaseBehavior;
            BehaviorBase.Cache.Add(instance);
            
            await instance.BuildAsync();
            return instance;
        }

        /// <summary>
        /// Calculate a server preformed skill
        /// </summary>
        /// <param name="associate">Executioner</param>
        /// <param name="writer">Data to be sent to clients</param>
        /// <param name="skillId">Skill to execute</param>
        /// <param name="syncId">Sync Id</param>
        /// <param name="calculatingPosition">Where position calculations are done from</param>
        /// <param name="target">Explicit target</param>
        /// <returns>Context</returns>
        public async Task<NpcExecutionContext> CalculateAsync(GameObject associate, BitWriter writer, int skillId,
            uint syncId, Vector3 calculatingPosition, GameObject target = default)
        {
            target ??= associate;

            var context = new NpcExecutionContext(target, writer, skillId, syncId, calculatingPosition);

            if (!SkillRoots.TryGetValue(skillId, out var root))
            {
                Logger.Debug($"Failed to find skill: {skillId}");
                return context;
            }

            context.Root = root;

            var branchContext = new ExecutionBranchContext { Target = target };

            try
            {
                await root.SerializeStart(context, branchContext);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return context;
        }

        public ExecutionContext Deserialize(GameObject associate, BitReader reader,
            SkillCastType castType = SkillCastType.OnEquip, GameObject target = default)
        {
            CastType = castType;
            
            var context = new ExecutionContext(associate, reader, default)
            {
                ExplicitTarget = target
            };

            DeserializeRootBehaviorsForSkillType(associate, SkillCastType.Default, context);
            DeserializeRootBehaviorsForSkillType(associate, castType, context);

            return context;
        }
        
        public async Task ExecuteAsync()
        {
            await ExecuteRootBehaviorsForSkillType(SkillCastType.Default);
            await ExecuteRootBehaviorsForSkillType(CastType);
        }

        private void DeserializeRootBehaviorsForSkillType(GameObject associate, SkillCastType skillType,
            ExecutionContext context)
        {
            if (RootBehaviors.TryGetValue(skillType, out var rootBehaviorList))
            {
                foreach (var executionPreparation in rootBehaviorList)
                {
                    // For each behavior specify behavior specific context (for example duration and explicit target)
                    context.Root = executionPreparation.BehaviorBase;
                    var branchContext = new ExecutionBranchContext { Target = associate };
                    
                    // Prepare the behavior for execution by populating it with context
                    executionPreparation.BehaviorExecutionParameters = 
                        executionPreparation.BehaviorBase.DeserializeStart(context, branchContext);
                }
            }
        }
        
        private async Task ExecuteRootBehaviorsForSkillType(SkillCastType skillType)
        {
            if (RootBehaviors.TryGetValue(skillType, out var rootBehaviorList))
            {
                foreach (var executionPreparation in rootBehaviorList)
                {
                    await executionPreparation.BehaviorBase.ExecuteStart(executionPreparation.BehaviorExecutionParameters);
                }
            }
        }

        public async Task<ExecutionContext> UseAsync(GameObject associate, BitReader reader, GameObject target)
        {
            var context = new ExecutionContext(associate, reader, default);

            if (!RootBehaviors.TryGetValue(SkillCastType.OnUse, out var list)) return context;

            foreach (var behaviorExecution in list)
            {
                await behaviorExecution.BehaviorBase.ExecuteStart(behaviorExecution.BehaviorExecutionParameters);
            }

            return context;
        }

        public async Task<ExecutionContext> MountAsync(GameObject associate)
        {
            var context = new ExecutionContext(associate, new BitReader(new MemoryStream()),
                new BitWriter(new MemoryStream()));

            if (!RootBehaviors.TryGetValue(SkillCastType.OnEquip, out var list)) return context;

            foreach (var executionPreparation in list)
            {
                await executionPreparation.BehaviorBase.ExecuteStart(executionPreparation.BehaviorExecutionParameters);
            }

            return context;
        }

        public async Task<ExecutionContext> DismantleAsync(GameObject associate)
        {
            var context = new ExecutionContext(associate, new BitReader(new MemoryStream()),
                new BitWriter(new MemoryStream()));

            if (!RootBehaviors.TryGetValue(SkillCastType.OnEquip, out var list)) return context;

            foreach (var executionPreparation in list)
            {
                await executionPreparation.BehaviorBase.DismantleAsync(executionPreparation.BehaviorExecutionParameters);
            }

            return context;
        }

        public static async Task<BehaviorInfo[]> GetSkillsForObject(Lot lot)
        {
            var tree = await FromLotAsync(lot);

            return tree.BehaviorIds;
        }
    }
}