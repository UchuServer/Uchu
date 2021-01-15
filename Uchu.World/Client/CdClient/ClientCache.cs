using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Systems.Missions;

namespace Uchu.World.Client
{
    /// <summary>
    /// Cache of the cd client context table
    /// </summary>
    public static class ClientCache
    {
        /// <summary>
        /// Cache of the Activities table in the cd client
        /// </summary>
        public static Activities[] ActivitiesTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the ActivityRewards table in the cd client
        /// </summary>
        public static ActivityRewards[] ActivityRewardsTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the Animations table in the cd client
        /// </summary>
        public static Animations[] AnimationsTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the BehaviorEffect table in the cd client
        /// </summary>
        public static BehaviorEffect[] BehaviorEffectTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the BehaviorParameter table in the cd client
        /// </summary>
        public static BehaviorParameter[] BehaviorParameterTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the BehaviorTemplate table in the cd client
        /// </summary>
        public static BehaviorTemplate[] BehaviorTemplateTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the BrickColors table in the cd client
        /// </summary>
        public static BrickColors[] BrickColorsTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the CelebrationParameters table in the cd client
        /// </summary>
        public static CelebrationParameters[] CelebrationParametersTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the ComponentsRegistry table in the cd client
        /// </summary>
        public static ComponentsRegistry[] ComponentsRegistryTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the CurrencyTable table in the cd client
        /// </summary>
        public static CurrencyTable[] CurrencyTableTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the DestructibleComponent table in the cd client
        /// </summary>
        public static Uchu.Core.Client.DestructibleComponent[] DestructibleComponentTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the Emotes table in the cd client
        /// </summary>
        public static Emotes[] EmotesTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the Emotes table in the cd client
        /// </summary>
        public static Factions[] FactionsTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the InventoryComponent table in the cd client
        /// </summary>
        public static Uchu.Core.Client.InventoryComponent[] InventoryComponentTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the ItemComponent table in the cd client
        /// </summary>
        public static ItemComponent[] ItemComponentTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the LevelProgressionLookup table in the cd client
        /// </summary>
        public static LevelProgressionLookup[] LevelProgressionLookupTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the LootMatrix table in the cd client
        /// </summary>
        public static LootMatrix[] LootMatrixTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the LootTable table in the cd client
        /// </summary>
        public static LootTable[] LootTableTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the MissionNPCComponent table in the cd client
        /// </summary>
        public static MissionNPCComponent[] MissionNPCComponentTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the MissionTasks table in the cd client
        /// </summary>
        public static MissionTasks[] MissionTasksTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the Missions table in the cd client
        /// </summary>
        public static Missions[] MissionsTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the MovementAIComponent table in the cd client
        /// </summary>
        public static MovementAIComponent[] MovementAIComponentTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the ObjectSkills table in the cd client
        /// </summary>
        public static ObjectSkills[] ObjectSkillsTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the Objects table in the cd client
        /// </summary>
        public static Objects[] ObjectsTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the PackageComponent table in the cd client
        /// </summary>
        public static PackageComponent[] PackageComponentTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the Preconditions table in the cd client
        /// </summary>
        public static Preconditions[] PreconditionsTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the  table in the cd client
        /// </summary>
        public static RebuildComponent[] RebuildComponentTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the RocketLaunchpadControlComponent table in the cd client
        /// </summary>
        public static RocketLaunchpadControlComponent[] RocketLaunchpadControlComponentTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the ScriptComponent table in the cd client
        /// </summary>
        public static ScriptComponent[] ScriptComponentTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the SkillBehavior table in the cd client
        /// </summary>
        public static SkillBehavior[] SkillBehaviorTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the VendorComponent table in the cd client
        /// </summary>
        public static Uchu.Core.Client.VendorComponent[] VendorComponentTable { get; private set; } = { };
        
        /// <summary>
        /// Cache of the ZoneTable table in the cd client
        /// </summary>
        public static ZoneTable[] ZoneTableTable { get; private set; } = { };

        /// <summary>
        /// All missions in the cd client
        /// </summary>
        public static MissionInstance[] Missions { get; private set; } = { };

        /// <summary>
        /// All achievements in the cd client
        /// </summary>
        public static MissionInstance[] Achievements { get; private set; } = { };

        public static async Task LoadAsync()
        {
            await using var cdContext = new CdClientContext();

            Logger.Debug("Setting up table caches");
            ActivitiesTable = cdContext.ActivitiesTable.ToArray();
            ActivityRewardsTable = cdContext.ActivityRewardsTable.ToArray();
            AnimationsTable = cdContext.AnimationsTable.ToArray();
            BehaviorEffectTable = cdContext.BehaviorEffectTable.ToArray();
            BehaviorParameterTable = cdContext.BehaviorParameterTable.ToArray();
            BehaviorTemplateTable = cdContext.BehaviorTemplateTable.ToArray();
            BrickColorsTable = cdContext.BrickColorsTable.ToArray();
            CelebrationParametersTable = cdContext.CelebrationParametersTable.ToArray();
            ComponentsRegistryTable = cdContext.ComponentsRegistryTable.ToArray();
            CurrencyTableTable = cdContext.CurrencyTableTable.ToArray();
            DestructibleComponentTable = cdContext.DestructibleComponentTable.ToArray();
            EmotesTable = cdContext.EmotesTable.ToArray();
            FactionsTable = cdContext.FactionsTable.ToArray();
            InventoryComponentTable = cdContext.InventoryComponentTable.ToArray();
            ItemComponentTable = cdContext.ItemComponentTable.ToArray();
            LevelProgressionLookupTable = cdContext.LevelProgressionLookupTable.ToArray();
            LootTableTable = cdContext.LootTableTable.ToArray();
            MissionNPCComponentTable = cdContext.MissionNPCComponentTable.ToArray();
            MissionTasksTable = cdContext.MissionTasksTable.ToArray();
            MissionsTable = cdContext.MissionsTable.ToArray();
            MovementAIComponentTable = cdContext.MovementAIComponentTable.ToArray();
            ObjectSkillsTable = cdContext.ObjectSkillsTable.ToArray();
            ObjectsTable = cdContext.ObjectsTable.ToArray();
            PackageComponentTable = cdContext.PackageComponentTable.ToArray();
            PreconditionsTable = cdContext.PreconditionsTable.ToArray();
            RebuildComponentTable = cdContext.RebuildComponentTable.ToArray();
            RocketLaunchpadControlComponentTable = cdContext.RocketLaunchpadControlComponentTable.ToArray();
            ScriptComponentTable = cdContext.ScriptComponentTable.ToArray();
            SkillBehaviorTable = cdContext.SkillBehaviorTable.ToArray();
            VendorComponentTable = cdContext.VendorComponentTable.ToArray();
            ZoneTableTable = cdContext.ZoneTableTable.ToArray();
            
            Logger.Debug("Setting up missions cache");
            Missions = cdContext.MissionsTable
                .ToArray()
                .Select(m =>
                {
                    var instance = new MissionInstance(m.Id ?? 0);
                    instance.Load();
                    return instance;
                }).ToArray();
            
            Achievements = Missions.Where(m => !m.IsMission).ToArray();
        }
    }
}