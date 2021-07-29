using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Uchu.Core.Client
{
	public class CdClientContext : DbContext, IAsyncDisposable
	{
		public DbSet<AICombatRoles> AICombatRolesTable { get; set; }

		public DbSet<AccessoryDefaultLoc> AccessoryDefaultLocTable { get; set; }

		public DbSet<Activities> ActivitiesTable { get; set; }

		public DbSet<ActivityRewards> ActivityRewardsTable { get; set; }

		public DbSet<ActivityText> ActivityTextTable { get; set; }

		public DbSet<AnimationIndex> AnimationIndexTable { get; set; }

		public DbSet<Animations> AnimationsTable { get; set; }

		public DbSet<BaseCombatAIComponent> BaseCombatAIComponentTable { get; set; }

		public DbSet<BehaviorEffect> BehaviorEffectTable { get; set; }

		public DbSet<BehaviorParameter> BehaviorParameterTable { get; set; }

		public DbSet<BehaviorTemplate> BehaviorTemplateTable { get; set; }

		public DbSet<BehaviorTemplateName> BehaviorTemplateNameTable { get; set; }

		public DbSet<Blueprints> BlueprintsTable { get; set; }

		public DbSet<BrickColors> BrickColorsTable { get; set; }

		public DbSet<BrickIDTable> BrickIDTableTable { get; set; }

		public DbSet<BuffDefinitions> BuffDefinitionsTable { get; set; }

		public DbSet<BuffParameters> BuffParametersTable { get; set; }

		public DbSet<Camera> CameraTable { get; set; }

		public DbSet<CelebrationParameters> CelebrationParametersTable { get; set; }

		public DbSet<ChoiceBuildComponent> ChoiceBuildComponentTable { get; set; }

		public DbSet<CollectibleComponent> CollectibleComponentTable { get; set; }

		public DbSet<ComponentsRegistry> ComponentsRegistryTable { get; set; }

		public DbSet<ControlSchemes> ControlSchemesTable { get; set; }

		public DbSet<CurrencyDenominations> CurrencyDenominationsTable { get; set; }

		public DbSet<CurrencyTable> CurrencyTableTable { get; set; }

		public DbSet<DBExclude> DBExcludeTable { get; set; }

		public DbSet<DeletionRestrictions> DeletionRestrictionsTable { get; set; }

		public DbSet<DestructibleComponent> DestructibleComponentTable { get; set; }

		public DbSet<DevModelBehaviors> DevModelBehaviorsTable { get; set; }

		public DbSet<Emotes> EmotesTable { get; set; }

		public DbSet<EventGating> EventGatingTable { get; set; }

		public DbSet<ExhibitComponent> ExhibitComponentTable { get; set; }

		public DbSet<Factions> FactionsTable { get; set; }

		public DbSet<FeatureGating> FeatureGatingTable { get; set; }

		public DbSet<FlairTable> FlairTableTable { get; set; }

		public DbSet<Icons> IconsTable { get; set; }

		public DbSet<InventoryComponent> InventoryComponentTable { get; set; }

		public DbSet<ItemComponent> ItemComponentTable { get; set; }

		public DbSet<ItemEggData> ItemEggDataTable { get; set; }

		public DbSet<ItemFoodData> ItemFoodDataTable { get; set; }

		public DbSet<ItemSetSkills> ItemSetSkillsTable { get; set; }

		public DbSet<ItemSets> ItemSetsTable { get; set; }

		public DbSet<JetPackPadComponent> JetPackPadComponentTable { get; set; }

		public DbSet<LUPExhibitComponent> LUPExhibitComponentTable { get; set; }

		public DbSet<LUPExhibitModelData> LUPExhibitModelDataTable { get; set; }

		public DbSet<LUPZoneIDs> LUPZoneIDsTable { get; set; }

		public DbSet<LanguageType> LanguageTypeTable { get; set; }

		public DbSet<LevelProgressionLookup> LevelProgressionLookupTable { get; set; }

		public DbSet<LootMatrix> LootMatrixTable { get; set; }

		public DbSet<LootMatrixIndex> LootMatrixIndexTable { get; set; }

		public DbSet<LootTable> LootTableTable { get; set; }

		public DbSet<LootTableIndex> LootTableIndexTable { get; set; }

		public DbSet<MinifigComponent> MinifigComponentTable { get; set; }

		public DbSet<MinifigDecalsEyebrows> MinifigDecalsEyebrowsTable { get; set; }

		public DbSet<MinifigDecalsEyes> MinifigDecalsEyesTable { get; set; }

		public DbSet<MinifigDecalsLegs> MinifigDecalsLegsTable { get; set; }

		public DbSet<MinifigDecalsMouths> MinifigDecalsMouthsTable { get; set; }

		public DbSet<MinifigDecalsTorsos> MinifigDecalsTorsosTable { get; set; }

		public DbSet<MissionEmail> MissionEmailTable { get; set; }

		public DbSet<MissionNPCComponent> MissionNPCComponentTable { get; set; }

		public DbSet<MissionTasks> MissionTasksTable { get; set; }

		public DbSet<MissionText> MissionTextTable { get; set; }

		public DbSet<Missions> MissionsTable { get; set; }

		public DbSet<ModelBehavior> ModelBehaviorTable { get; set; }

		public DbSet<ModularBuildComponent> ModularBuildComponentTable { get; set; }

		public DbSet<ModuleComponent> ModuleComponentTable { get; set; }

		public DbSet<MotionFX> MotionFXTable { get; set; }

		public DbSet<MovementAIComponent> MovementAIComponentTable { get; set; }

		public DbSet<MovingPlatforms> MovingPlatformsTable { get; set; }

		public DbSet<NpcIcons> NpcIconsTable { get; set; }

		public DbSet<ObjectBehaviorXREF> ObjectBehaviorXREFTable { get; set; }

		public DbSet<ObjectBehaviors> ObjectBehaviorsTable { get; set; }

		public DbSet<ObjectSkills> ObjectSkillsTable { get; set; }

		public DbSet<Objects> ObjectsTable { get; set; }

		public DbSet<PackageComponent> PackageComponentTable { get; set; }

		public DbSet<PetAbilities> PetAbilitiesTable { get; set; }

		public DbSet<PetComponent> PetComponentTable { get; set; }

		public DbSet<PetNestComponent> PetNestComponentTable { get; set; }

		public DbSet<PhysicsComponent> PhysicsComponentTable { get; set; }

		public DbSet<PlayerFlags> PlayerFlagsTable { get; set; }

		public DbSet<PlayerStatistics> PlayerStatisticsTable { get; set; }

		public DbSet<PossessableComponent> PossessableComponentTable { get; set; }

		public DbSet<Preconditions> PreconditionsTable { get; set; }

		public DbSet<PropertyEntranceComponent> PropertyEntranceComponentTable { get; set; }

		public DbSet<PropertyTemplate> PropertyTemplateTable { get; set; }

		public DbSet<ProximityMonitorComponent> ProximityMonitorComponentTable { get; set; }

		public DbSet<ProximityTypes> ProximityTypesTable { get; set; }

		public DbSet<RacingModuleComponent> RacingModuleComponentTable { get; set; }

		public DbSet<RailActivatorComponent> RailActivatorComponentTable { get; set; }

		public DbSet<RarityTable> RarityTableTable { get; set; }

		public DbSet<RarityTableIndex> RarityTableIndexTable { get; set; }

		public DbSet<RebuildComponent> RebuildComponentTable { get; set; }

		public DbSet<RebuildSections> RebuildSectionsTable { get; set; }

		public DbSet<ReleaseVersion> ReleaseVersionTable { get; set; }

		public DbSet<RenderComponent> RenderComponentTable { get; set; }

		public DbSet<RenderComponentFlash> RenderComponentFlashTable { get; set; }

		public DbSet<RenderComponentWrapper> RenderComponentWrapperTable { get; set; }

		public DbSet<RenderIconAssets> RenderIconAssetsTable { get; set; }

		public DbSet<ReputationRewards> ReputationRewardsTable { get; set; }

		public DbSet<RewardCodes> RewardCodesTable { get; set; }

		public DbSet<Rewards> RewardsTable { get; set; }

		public DbSet<RocketLaunchpadControlComponent> RocketLaunchpadControlComponentTable { get; set; }

		public DbSet<SceneTable> SceneTableTable { get; set; }

		public DbSet<ScriptComponent> ScriptComponentTable { get; set; }

		public DbSet<SkillBehavior> SkillBehaviorTable { get; set; }

		public DbSet<SmashableChain> SmashableChainTable { get; set; }

		public DbSet<SmashableChainIndex> SmashableChainIndexTable { get; set; }

		public DbSet<SmashableComponent> SmashableComponentTable { get; set; }

		public DbSet<SmashableElements> SmashableElementsTable { get; set; }

		public DbSet<SpeedchatMenu> SpeedchatMenuTable { get; set; }

		public DbSet<SubscriptionPricing> SubscriptionPricingTable { get; set; }

		public DbSet<SurfaceType> SurfaceTypeTable { get; set; }

		public DbSet<TamingBuildPuzzles> TamingBuildPuzzlesTable { get; set; }

		public DbSet<TextDescription> TextDescriptionTable { get; set; }

		public DbSet<TextLanguage> TextLanguageTable { get; set; }

		public DbSet<TrailEffects> TrailEffectsTable { get; set; }

		public DbSet<UGBehaviorSounds> UGBehaviorSoundsTable { get; set; }

		public DbSet<VehiclePhysics> VehiclePhysicsTable { get; set; }

		public DbSet<VehicleStatMap> VehicleStatMapTable { get; set; }

		public DbSet<VendorComponent> VendorComponentTable { get; set; }

		public DbSet<WhatsCoolItemSpotlight> WhatsCoolItemSpotlightTable { get; set; }

		public DbSet<WhatsCoolNewsAndTips> WhatsCoolNewsAndTipsTable { get; set; }

		public DbSet<WorldConfig> WorldConfigTable { get; set; }

		public DbSet<ZoneLoadingTips> ZoneLoadingTipsTable { get; set; }

		public DbSet<ZoneSummary> ZoneSummaryTable { get; set; }

		public DbSet<ZoneTable> ZoneTableTable { get; set; }

		public DbSet<brickAttributes> brickAttributesTable { get; set; }

		public DbSet<dtproperties> dtpropertiesTable { get; set; }

		public DbSet<mapAnimationPriorities> mapAnimationPrioritiesTable { get; set; }

		public DbSet<mapAssetType> mapAssetTypeTable { get; set; }

		public DbSet<mapIcon> mapIconTable { get; set; }

		public DbSet<mapItemTypes> mapItemTypesTable { get; set; }

		public DbSet<mapRenderEffects> mapRenderEffectsTable { get; set; }

		public DbSet<mapShaders> mapShadersTable { get; set; }

		public DbSet<mapTextureResource> mapTextureResourceTable { get; set; }

		public DbSet<mapBlueprintCategory> mapBlueprintCategoryTable { get; set; }

		public DbSet<sysdiagrams> sysdiagramsTable { get; set; }

		public async Task EnsureUpdatedAsync()
		{
			await Database.MigrateAsync().ConfigureAwait(false);
		}
		
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Data Source=./CDClient.db");
		}

		public override ValueTask DisposeAsync()
		{
			return new ValueTask(Task.CompletedTask);
		}
	}
}