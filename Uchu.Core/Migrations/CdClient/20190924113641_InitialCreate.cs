using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations.CdClient
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessoryDefaultLoc",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupID = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Pos_X = table.Column<float>(nullable: true),
                    Pos_Y = table.Column<float>(nullable: true),
                    Pos_Z = table.Column<float>(nullable: true),
                    Rot_X = table.Column<float>(nullable: true),
                    Rot_Y = table.Column<float>(nullable: true),
                    Rot_Z = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessoryDefaultLoc", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityID = table.Column<int>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    instanceMapID = table.Column<int>(nullable: true),
                    minTeams = table.Column<int>(nullable: true),
                    maxTeams = table.Column<int>(nullable: true),
                    minTeamSize = table.Column<int>(nullable: true),
                    maxTeamSize = table.Column<int>(nullable: true),
                    waitTime = table.Column<int>(nullable: true),
                    startDelay = table.Column<int>(nullable: true),
                    requiresUniqueData = table.Column<bool>(nullable: true),
                    leaderboardType = table.Column<int>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    optionalCostLOT = table.Column<int>(nullable: true),
                    optionalCostCount = table.Column<int>(nullable: true),
                    showUIRewards = table.Column<bool>(nullable: true),
                    CommunityActivityFlagID = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true),
                    noTeamLootOnDeath = table.Column<bool>(nullable: true),
                    optionalPercentage = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ActivityRewards",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    objectTemplate = table.Column<int>(nullable: true),
                    ActivityRewardIndex = table.Column<int>(nullable: true),
                    activityRating = table.Column<int>(nullable: true),
                    LootMatrixIndex = table.Column<int>(nullable: true),
                    CurrencyIndex = table.Column<int>(nullable: true),
                    ChallengeRating = table.Column<int>(nullable: true),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRewards", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ActivityText",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    activityID = table.Column<int>(nullable: true),
                    type = table.Column<string>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityText", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "AICombatRoles",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    preferredRole = table.Column<int>(nullable: true),
                    specifiedMinRangeNOUSE = table.Column<float>(nullable: true),
                    specifiedMaxRangeNOUSE = table.Column<float>(nullable: true),
                    specificMinRange = table.Column<float>(nullable: true),
                    specificMaxRange = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AICombatRoles", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "AnimationIndex",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    animationGroupID = table.Column<int>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    groupType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimationIndex", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Animations",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    animationGroupID = table.Column<int>(nullable: true),
                    animation_type = table.Column<string>(nullable: true),
                    animation_name = table.Column<string>(nullable: true),
                    chance_to_play = table.Column<float>(nullable: true),
                    min_loops = table.Column<int>(nullable: true),
                    max_loops = table.Column<int>(nullable: true),
                    animation_length = table.Column<float>(nullable: true),
                    hideEquip = table.Column<bool>(nullable: true),
                    ignoreUpperBody = table.Column<bool>(nullable: true),
                    restartable = table.Column<bool>(nullable: true),
                    face_animation_name = table.Column<string>(nullable: true),
                    priority = table.Column<float>(nullable: true),
                    blendTime = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animations", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "BaseCombatAIComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    behaviorType = table.Column<int>(nullable: true),
                    combatRoundLength = table.Column<float>(nullable: true),
                    combatRole = table.Column<int>(nullable: true),
                    minRoundLength = table.Column<float>(nullable: true),
                    maxRoundLength = table.Column<float>(nullable: true),
                    tetherSpeed = table.Column<float>(nullable: true),
                    pursuitSpeed = table.Column<float>(nullable: true),
                    combatStartDelay = table.Column<float>(nullable: true),
                    softTetherRadius = table.Column<float>(nullable: true),
                    hardTetherRadius = table.Column<float>(nullable: true),
                    spawnTimer = table.Column<float>(nullable: true),
                    tetherEffectID = table.Column<int>(nullable: true),
                    ignoreMediator = table.Column<bool>(nullable: true),
                    aggroRadius = table.Column<float>(nullable: true),
                    ignoreStatReset = table.Column<bool>(nullable: true),
                    ignoreParent = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseCombatAIComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "BehaviorEffect",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    effectID = table.Column<int>(nullable: true),
                    effectType = table.Column<string>(nullable: true),
                    effectName = table.Column<string>(nullable: true),
                    trailID = table.Column<int>(nullable: true),
                    pcreateDuration = table.Column<float>(nullable: true),
                    animationName = table.Column<string>(nullable: true),
                    attachToObject = table.Column<bool>(nullable: true),
                    boneName = table.Column<string>(nullable: true),
                    useSecondary = table.Column<bool>(nullable: true),
                    cameraEffectType = table.Column<int>(nullable: true),
                    cameraDuration = table.Column<float>(nullable: true),
                    cameraFrequency = table.Column<float>(nullable: true),
                    cameraXAmp = table.Column<float>(nullable: true),
                    cameraYAmp = table.Column<float>(nullable: true),
                    cameraZAmp = table.Column<float>(nullable: true),
                    cameraRotFrequency = table.Column<float>(nullable: true),
                    cameraRoll = table.Column<float>(nullable: true),
                    cameraPitch = table.Column<float>(nullable: true),
                    cameraYaw = table.Column<float>(nullable: true),
                    AudioEventGUID = table.Column<string>(nullable: true),
                    renderEffectType = table.Column<int>(nullable: true),
                    renderEffectTime = table.Column<float>(nullable: true),
                    renderStartVal = table.Column<float>(nullable: true),
                    renderEndVal = table.Column<float>(nullable: true),
                    renderDelayVal = table.Column<float>(nullable: true),
                    renderValue1 = table.Column<float>(nullable: true),
                    renderValue2 = table.Column<float>(nullable: true),
                    renderValue3 = table.Column<float>(nullable: true),
                    renderRGBA = table.Column<string>(nullable: true),
                    renderShaderVal = table.Column<int>(nullable: true),
                    motionID = table.Column<int>(nullable: true),
                    meshID = table.Column<int>(nullable: true),
                    meshDuration = table.Column<float>(nullable: true),
                    meshLockedNode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehaviorEffect", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "BehaviorParameter",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    behaviorID = table.Column<int>(nullable: true),
                    parameterID = table.Column<string>(nullable: true),
                    value = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehaviorParameter", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "BehaviorTemplate",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    behaviorID = table.Column<int>(nullable: true),
                    templateID = table.Column<int>(nullable: true),
                    effectID = table.Column<int>(nullable: true),
                    effectHandle = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehaviorTemplate", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "BehaviorTemplateName",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    templateID = table.Column<int>(nullable: true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehaviorTemplateName", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Blueprints",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<long>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    accountid = table.Column<long>(nullable: true),
                    characterid = table.Column<long>(nullable: true),
                    price = table.Column<int>(nullable: true),
                    rating = table.Column<int>(nullable: true),
                    categoryid = table.Column<int>(nullable: true),
                    lxfpath = table.Column<string>(nullable: true),
                    deleted = table.Column<bool>(nullable: true),
                    created = table.Column<long>(nullable: true),
                    modified = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blueprints", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "brickAttributes",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID = table.Column<int>(nullable: true),
                    icon_asset = table.Column<string>(nullable: true),
                    display_order = table.Column<int>(nullable: true),
                    locStatus = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brickAttributes", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "BrickColors",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    red = table.Column<float>(nullable: true),
                    green = table.Column<float>(nullable: true),
                    blue = table.Column<float>(nullable: true),
                    alpha = table.Column<float>(nullable: true),
                    legopaletteid = table.Column<int>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    validTypes = table.Column<int>(nullable: true),
                    validCharacters = table.Column<int>(nullable: true),
                    factoryValid = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrickColors", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "BrickIDTable",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NDObjectID = table.Column<int>(nullable: true),
                    LEGOBrickID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrickIDTable", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "BuffDefinitions",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID = table.Column<int>(nullable: true),
                    Priority = table.Column<float>(nullable: true),
                    UIIcon = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuffDefinitions", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "BuffParameters",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuffID = table.Column<int>(nullable: true),
                    ParameterName = table.Column<string>(nullable: true),
                    NumberValue = table.Column<float>(nullable: true),
                    StringValue = table.Column<string>(nullable: true),
                    EffectID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuffParameters", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Camera",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    camera_name = table.Column<string>(nullable: true),
                    pitch_angle_tolerance = table.Column<float>(nullable: true),
                    starting_zoom = table.Column<float>(nullable: true),
                    zoom_return_modifier = table.Column<float>(nullable: true),
                    pitch_return_modifier = table.Column<float>(nullable: true),
                    tether_out_return_modifier = table.Column<float>(nullable: true),
                    tether_in_return_multiplier = table.Column<float>(nullable: true),
                    verticle_movement_dampening_modifier = table.Column<float>(nullable: true),
                    return_from_incline_modifier = table.Column<float>(nullable: true),
                    horizontal_return_modifier = table.Column<float>(nullable: true),
                    yaw_behavior_speed_multiplier = table.Column<float>(nullable: true),
                    camera_collision_padding = table.Column<float>(nullable: true),
                    glide_speed = table.Column<float>(nullable: true),
                    fade_player_min_range = table.Column<float>(nullable: true),
                    min_movement_delta_tolerance = table.Column<float>(nullable: true),
                    min_glide_distance_tolerance = table.Column<float>(nullable: true),
                    look_forward_offset = table.Column<float>(nullable: true),
                    look_up_offset = table.Column<float>(nullable: true),
                    minimum_vertical_dampening_distance = table.Column<float>(nullable: true),
                    maximum_vertical_dampening_distance = table.Column<float>(nullable: true),
                    minimum_ignore_jump_distance = table.Column<float>(nullable: true),
                    maximum_ignore_jump_distance = table.Column<float>(nullable: true),
                    maximum_auto_glide_angle = table.Column<float>(nullable: true),
                    minimum_tether_glide_distance = table.Column<float>(nullable: true),
                    yaw_sign_correction = table.Column<float>(nullable: true),
                    set_1_look_forward_offset = table.Column<float>(nullable: true),
                    set_1_look_up_offset = table.Column<float>(nullable: true),
                    set_2_look_forward_offset = table.Column<float>(nullable: true),
                    set_2_look_up_offset = table.Column<float>(nullable: true),
                    set_0_speed_influence_on_dir = table.Column<float>(nullable: true),
                    set_1_speed_influence_on_dir = table.Column<float>(nullable: true),
                    set_2_speed_influence_on_dir = table.Column<float>(nullable: true),
                    set_0_angular_relaxation = table.Column<float>(nullable: true),
                    set_1_angular_relaxation = table.Column<float>(nullable: true),
                    set_2_angular_relaxation = table.Column<float>(nullable: true),
                    set_0_position_up_offset = table.Column<float>(nullable: true),
                    set_1_position_up_offset = table.Column<float>(nullable: true),
                    set_2_position_up_offset = table.Column<float>(nullable: true),
                    set_0_position_forward_offset = table.Column<float>(nullable: true),
                    set_1_position_forward_offset = table.Column<float>(nullable: true),
                    set_2_position_forward_offset = table.Column<float>(nullable: true),
                    set_0_FOV = table.Column<float>(nullable: true),
                    set_1_FOV = table.Column<float>(nullable: true),
                    set_2_FOV = table.Column<float>(nullable: true),
                    set_0_max_yaw_angle = table.Column<float>(nullable: true),
                    set_1_max_yaw_angle = table.Column<float>(nullable: true),
                    set_2_max_yaw_angle = table.Column<float>(nullable: true),
                    set_1_fade_in_camera_set_change = table.Column<int>(nullable: true),
                    set_1_fade_out_camera_set_change = table.Column<int>(nullable: true),
                    set_2_fade_in_camera_set_change = table.Column<int>(nullable: true),
                    set_2_fade_out_camera_set_change = table.Column<int>(nullable: true),
                    input_movement_scalar = table.Column<float>(nullable: true),
                    input_rotation_scalar = table.Column<float>(nullable: true),
                    input_zoom_scalar = table.Column<float>(nullable: true),
                    minimum_pitch_desired = table.Column<float>(nullable: true),
                    maximum_pitch_desired = table.Column<float>(nullable: true),
                    minimum_zoom = table.Column<float>(nullable: true),
                    maximum_zoom = table.Column<float>(nullable: true),
                    horizontal_rotate_tolerance = table.Column<float>(nullable: true),
                    horizontal_rotate_modifier = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Camera", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "CelebrationParameters",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    animation = table.Column<string>(nullable: true),
                    backgroundObject = table.Column<int>(nullable: true),
                    duration = table.Column<float>(nullable: true),
                    subText = table.Column<string>(nullable: true),
                    mainText = table.Column<string>(nullable: true),
                    iconID = table.Column<int>(nullable: true),
                    celeLeadIn = table.Column<float>(nullable: true),
                    celeLeadOut = table.Column<float>(nullable: true),
                    cameraPathLOT = table.Column<int>(nullable: true),
                    pathNodeName = table.Column<string>(nullable: true),
                    ambientR = table.Column<float>(nullable: true),
                    ambientG = table.Column<float>(nullable: true),
                    ambientB = table.Column<float>(nullable: true),
                    directionalR = table.Column<float>(nullable: true),
                    directionalG = table.Column<float>(nullable: true),
                    directionalB = table.Column<float>(nullable: true),
                    specularR = table.Column<float>(nullable: true),
                    specularG = table.Column<float>(nullable: true),
                    specularB = table.Column<float>(nullable: true),
                    lightPositionX = table.Column<float>(nullable: true),
                    lightPositionY = table.Column<float>(nullable: true),
                    lightPositionZ = table.Column<float>(nullable: true),
                    blendTime = table.Column<float>(nullable: true),
                    fogColorR = table.Column<float>(nullable: true),
                    fogColorG = table.Column<float>(nullable: true),
                    fogColorB = table.Column<float>(nullable: true),
                    musicCue = table.Column<string>(nullable: true),
                    soundGUID = table.Column<string>(nullable: true),
                    mixerProgram = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CelebrationParameters", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ChoiceBuildComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    selections = table.Column<string>(nullable: true),
                    imaginationOverride = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChoiceBuildComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "CollectibleComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    requirement_mission = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectibleComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ComponentsRegistry",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    component_type = table.Column<int>(nullable: true),
                    component_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentsRegistry", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ControlSchemes",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    control_scheme = table.Column<int>(nullable: true),
                    scheme_name = table.Column<string>(nullable: true),
                    rotation_speed = table.Column<float>(nullable: true),
                    walk_forward_speed = table.Column<float>(nullable: true),
                    walk_backward_speed = table.Column<float>(nullable: true),
                    walk_strafe_speed = table.Column<float>(nullable: true),
                    walk_strafe_forward_speed = table.Column<float>(nullable: true),
                    walk_strafe_backward_speed = table.Column<float>(nullable: true),
                    run_backward_speed = table.Column<float>(nullable: true),
                    run_strafe_speed = table.Column<float>(nullable: true),
                    run_strafe_forward_speed = table.Column<float>(nullable: true),
                    run_strafe_backward_speed = table.Column<float>(nullable: true),
                    keyboard_zoom_sensitivity = table.Column<float>(nullable: true),
                    keyboard_pitch_sensitivity = table.Column<float>(nullable: true),
                    keyboard_yaw_sensitivity = table.Column<float>(nullable: true),
                    mouse_zoom_wheel_sensitivity = table.Column<float>(nullable: true),
                    x_mouse_move_sensitivity_modifier = table.Column<float>(nullable: true),
                    y_mouse_move_sensitivity_modifier = table.Column<float>(nullable: true),
                    freecam_speed_modifier = table.Column<float>(nullable: true),
                    freecam_slow_speed_multiplier = table.Column<float>(nullable: true),
                    freecam_fast_speed_multiplier = table.Column<float>(nullable: true),
                    freecam_mouse_modifier = table.Column<float>(nullable: true),
                    gamepad_pitch_rot_sensitivity = table.Column<float>(nullable: true),
                    gamepad_yaw_rot_sensitivity = table.Column<float>(nullable: true),
                    gamepad_trigger_sensitivity = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlSchemes", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyDenominations",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    value = table.Column<int>(nullable: true),
                    objectid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyDenominations", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyTable",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    currencyIndex = table.Column<int>(nullable: true),
                    npcminlevel = table.Column<int>(nullable: true),
                    minvalue = table.Column<int>(nullable: true),
                    maxvalue = table.Column<int>(nullable: true),
                    id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyTable", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "DBExclude",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    table = table.Column<string>(nullable: true),
                    column = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBExclude", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "DeletionRestrictions",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    restricted = table.Column<bool>(nullable: true),
                    ids = table.Column<string>(nullable: true),
                    checkType = table.Column<int>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletionRestrictions", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "DestructibleComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    faction = table.Column<int>(nullable: true),
                    factionList = table.Column<string>(nullable: true),
                    life = table.Column<int>(nullable: true),
                    imagination = table.Column<int>(nullable: true),
                    LootMatrixIndex = table.Column<int>(nullable: true),
                    CurrencyIndex = table.Column<int>(nullable: true),
                    level = table.Column<int>(nullable: true),
                    armor = table.Column<float>(nullable: true),
                    death_behavior = table.Column<int>(nullable: true),
                    isnpc = table.Column<bool>(nullable: true),
                    attack_priority = table.Column<int>(nullable: true),
                    isSmashable = table.Column<bool>(nullable: true),
                    difficultyLevel = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestructibleComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "DevModelBehaviors",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModelID = table.Column<int>(nullable: true),
                    BehaviorID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevModelBehaviors", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "dtproperties",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    objectid = table.Column<int>(nullable: true),
                    property = table.Column<string>(nullable: true),
                    value = table.Column<string>(nullable: true),
                    uvalue = table.Column<string>(nullable: true),
                    lvalue = table.Column<int>(nullable: true),
                    version = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dtproperties", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Emotes",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    animationName = table.Column<string>(nullable: true),
                    iconFilename = table.Column<string>(nullable: true),
                    channel = table.Column<string>(nullable: true),
                    command = table.Column<string>(nullable: true),
                    locked = table.Column<bool>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emotes", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "EventGating",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    eventName = table.Column<string>(nullable: true),
                    date_start = table.Column<long>(nullable: true),
                    date_end = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventGating", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ExhibitComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    length = table.Column<float>(nullable: true),
                    width = table.Column<float>(nullable: true),
                    height = table.Column<float>(nullable: true),
                    offsetX = table.Column<float>(nullable: true),
                    offsetY = table.Column<float>(nullable: true),
                    offsetZ = table.Column<float>(nullable: true),
                    fReputationSizeMultiplier = table.Column<float>(nullable: true),
                    fImaginationCost = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExhibitComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Factions",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    faction = table.Column<int>(nullable: true),
                    factionList = table.Column<string>(nullable: true),
                    factionListFriendly = table.Column<bool>(nullable: true),
                    friendList = table.Column<string>(nullable: true),
                    enemyList = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factions", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "FeatureGating",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    featureName = table.Column<string>(nullable: true),
                    major = table.Column<int>(nullable: true),
                    current = table.Column<int>(nullable: true),
                    minor = table.Column<int>(nullable: true),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureGating", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "FlairTable",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    asset = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlairTable", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Icons",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IconID = table.Column<int>(nullable: true),
                    IconPath = table.Column<string>(nullable: true),
                    IconName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icons", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "InventoryComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    itemid = table.Column<int>(nullable: true),
                    count = table.Column<int>(nullable: true),
                    equip = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ItemComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    equipLocation = table.Column<string>(nullable: true),
                    baseValue = table.Column<int>(nullable: true),
                    isKitPiece = table.Column<bool>(nullable: true),
                    rarity = table.Column<int>(nullable: true),
                    itemType = table.Column<int>(nullable: true),
                    itemInfo = table.Column<long>(nullable: true),
                    inLootTable = table.Column<bool>(nullable: true),
                    inVendor = table.Column<bool>(nullable: true),
                    isUnique = table.Column<bool>(nullable: true),
                    isBOP = table.Column<bool>(nullable: true),
                    isBOE = table.Column<bool>(nullable: true),
                    reqFlagID = table.Column<int>(nullable: true),
                    reqSpecialtyID = table.Column<int>(nullable: true),
                    reqSpecRank = table.Column<int>(nullable: true),
                    reqAchievementID = table.Column<int>(nullable: true),
                    stackSize = table.Column<int>(nullable: true),
                    color1 = table.Column<int>(nullable: true),
                    decal = table.Column<int>(nullable: true),
                    offsetGroupID = table.Column<int>(nullable: true),
                    buildTypes = table.Column<int>(nullable: true),
                    reqPrecondition = table.Column<string>(nullable: true),
                    animationFlag = table.Column<int>(nullable: true),
                    equipEffects = table.Column<int>(nullable: true),
                    readyForQA = table.Column<bool>(nullable: true),
                    itemRating = table.Column<int>(nullable: true),
                    isTwoHanded = table.Column<bool>(nullable: true),
                    minNumRequired = table.Column<int>(nullable: true),
                    delResIndex = table.Column<int>(nullable: true),
                    currencyLOT = table.Column<int>(nullable: true),
                    altCurrencyCost = table.Column<int>(nullable: true),
                    subItems = table.Column<string>(nullable: true),
                    audioEventUse = table.Column<string>(nullable: true),
                    noEquipAnimation = table.Column<bool>(nullable: true),
                    commendationLOT = table.Column<int>(nullable: true),
                    commendationCost = table.Column<int>(nullable: true),
                    audioEquipMetaEventSet = table.Column<string>(nullable: true),
                    currencyCosts = table.Column<string>(nullable: true),
                    ingredientInfo = table.Column<string>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    forgeType = table.Column<int>(nullable: true),
                    SellMultiplier = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ItemEggData",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    chassie_type_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemEggData", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ItemFoodData",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    element_1 = table.Column<int>(nullable: true),
                    element_1_amount = table.Column<int>(nullable: true),
                    element_2 = table.Column<int>(nullable: true),
                    element_2_amount = table.Column<int>(nullable: true),
                    element_3 = table.Column<int>(nullable: true),
                    element_3_amount = table.Column<int>(nullable: true),
                    element_4 = table.Column<int>(nullable: true),
                    element_4_amount = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemFoodData", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ItemSets",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    setID = table.Column<int>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    itemIDs = table.Column<string>(nullable: true),
                    kitType = table.Column<int>(nullable: true),
                    kitRank = table.Column<int>(nullable: true),
                    kitImage = table.Column<int>(nullable: true),
                    skillSetWith2 = table.Column<int>(nullable: true),
                    skillSetWith3 = table.Column<int>(nullable: true),
                    skillSetWith4 = table.Column<int>(nullable: true),
                    skillSetWith5 = table.Column<int>(nullable: true),
                    skillSetWith6 = table.Column<int>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    gate_version = table.Column<string>(nullable: true),
                    kitID = table.Column<int>(nullable: true),
                    priority = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSets", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ItemSetSkills",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SkillSetID = table.Column<int>(nullable: true),
                    SkillID = table.Column<int>(nullable: true),
                    SkillCastType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSetSkills", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "JetPackPadComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    xDistance = table.Column<float>(nullable: true),
                    yDistance = table.Column<float>(nullable: true),
                    warnDistance = table.Column<float>(nullable: true),
                    lotBlocker = table.Column<int>(nullable: true),
                    lotWarningVolume = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JetPackPadComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "LanguageType",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LanguageID = table.Column<int>(nullable: true),
                    LanguageDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageType", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "LevelProgressionLookup",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    requiredUScore = table.Column<int>(nullable: true),
                    BehaviorEffect = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelProgressionLookup", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "LootMatrix",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LootMatrixIndex = table.Column<int>(nullable: true),
                    LootTableIndex = table.Column<int>(nullable: true),
                    RarityTableIndex = table.Column<int>(nullable: true),
                    percent = table.Column<float>(nullable: true),
                    minToDrop = table.Column<int>(nullable: true),
                    maxToDrop = table.Column<int>(nullable: true),
                    id = table.Column<int>(nullable: true),
                    flagID = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootMatrix", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "LootMatrixIndex",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LootMatrixIndex = table.Column<int>(nullable: true),
                    inNpcEditor = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootMatrixIndex", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "LootTable",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    itemid = table.Column<int>(nullable: true),
                    LootTableIndex = table.Column<int>(nullable: true),
                    id = table.Column<int>(nullable: true),
                    MissionDrop = table.Column<bool>(nullable: true),
                    sortPriority = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootTable", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "LootTableIndex",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LootTableIndex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootTableIndex", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "LUPExhibitComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    minXZ = table.Column<float>(nullable: true),
                    maxXZ = table.Column<float>(nullable: true),
                    maxY = table.Column<float>(nullable: true),
                    offsetX = table.Column<float>(nullable: true),
                    offsetY = table.Column<float>(nullable: true),
                    offsetZ = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LUPExhibitComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "LUPExhibitModelData",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LOT = table.Column<int>(nullable: true),
                    minXZ = table.Column<float>(nullable: true),
                    maxXZ = table.Column<float>(nullable: true),
                    maxY = table.Column<float>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    owner = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LUPExhibitModelData", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "LUPZoneIDs",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    zoneID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LUPZoneIDs", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "map_BlueprintCategory",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    enabled = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map_BlueprintCategory", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "mapAnimationPriorities",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    priority = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapAnimationPriorities", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "mapAssetType",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    label = table.Column<string>(nullable: true),
                    pathdir = table.Column<string>(nullable: true),
                    typelabel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapAssetType", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "mapIcon",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LOT = table.Column<int>(nullable: true),
                    iconID = table.Column<int>(nullable: true),
                    iconState = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapIcon", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "mapItemTypes",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    equipLocation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapItemTypes", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "mapRenderEffects",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    gameID = table.Column<int>(nullable: true),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapRenderEffects", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "mapShaders",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    label = table.Column<string>(nullable: true),
                    gameValue = table.Column<int>(nullable: true),
                    priority = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapShaders", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "mapTextureResource",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    texturepath = table.Column<string>(nullable: true),
                    SurfaceType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapTextureResource", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MinifigComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    head = table.Column<int>(nullable: true),
                    chest = table.Column<int>(nullable: true),
                    legs = table.Column<int>(nullable: true),
                    hairstyle = table.Column<int>(nullable: true),
                    haircolor = table.Column<int>(nullable: true),
                    chestdecal = table.Column<int>(nullable: true),
                    headcolor = table.Column<int>(nullable: true),
                    lefthand = table.Column<int>(nullable: true),
                    righthand = table.Column<int>(nullable: true),
                    eyebrowstyle = table.Column<int>(nullable: true),
                    eyesstyle = table.Column<int>(nullable: true),
                    mouthstyle = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinifigComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MinifigDecals_Eyebrows",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID = table.Column<int>(nullable: true),
                    High_path = table.Column<string>(nullable: true),
                    Low_path = table.Column<string>(nullable: true),
                    CharacterCreateValid = table.Column<bool>(nullable: true),
                    male = table.Column<bool>(nullable: true),
                    female = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinifigDecals_Eyebrows", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MinifigDecals_Eyes",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID = table.Column<int>(nullable: true),
                    High_path = table.Column<string>(nullable: true),
                    Low_path = table.Column<string>(nullable: true),
                    CharacterCreateValid = table.Column<bool>(nullable: true),
                    male = table.Column<bool>(nullable: true),
                    female = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinifigDecals_Eyes", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MinifigDecals_Legs",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID = table.Column<int>(nullable: true),
                    High_path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinifigDecals_Legs", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MinifigDecals_Mouths",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID = table.Column<int>(nullable: true),
                    High_path = table.Column<string>(nullable: true),
                    Low_path = table.Column<string>(nullable: true),
                    CharacterCreateValid = table.Column<bool>(nullable: true),
                    male = table.Column<bool>(nullable: true),
                    female = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinifigDecals_Mouths", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MinifigDecals_Torsos",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID = table.Column<int>(nullable: true),
                    High_path = table.Column<string>(nullable: true),
                    CharacterCreateValid = table.Column<bool>(nullable: true),
                    male = table.Column<bool>(nullable: true),
                    female = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinifigDecals_Torsos", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MissionEmail",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID = table.Column<int>(nullable: true),
                    messageType = table.Column<int>(nullable: true),
                    notificationGroup = table.Column<int>(nullable: true),
                    missionID = table.Column<int>(nullable: true),
                    attachmentLOT = table.Column<int>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionEmail", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MissionNPCComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    missionID = table.Column<int>(nullable: true),
                    offersMission = table.Column<bool>(nullable: true),
                    acceptsMission = table.Column<bool>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionNPCComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    defined_type = table.Column<string>(nullable: true),
                    defined_subtype = table.Column<string>(nullable: true),
                    UISortOrder = table.Column<int>(nullable: true),
                    offer_objectID = table.Column<int>(nullable: true),
                    target_objectID = table.Column<int>(nullable: true),
                    reward_currency = table.Column<long>(nullable: true),
                    LegoScore = table.Column<int>(nullable: true),
                    reward_reputation = table.Column<long>(nullable: true),
                    isChoiceReward = table.Column<bool>(nullable: true),
                    reward_item1 = table.Column<int>(nullable: true),
                    reward_item1_count = table.Column<int>(nullable: true),
                    reward_item2 = table.Column<int>(nullable: true),
                    reward_item2_count = table.Column<int>(nullable: true),
                    reward_item3 = table.Column<int>(nullable: true),
                    reward_item3_count = table.Column<int>(nullable: true),
                    reward_item4 = table.Column<int>(nullable: true),
                    reward_item4_count = table.Column<int>(nullable: true),
                    reward_emote = table.Column<int>(nullable: true),
                    reward_emote2 = table.Column<int>(nullable: true),
                    reward_emote3 = table.Column<int>(nullable: true),
                    reward_emote4 = table.Column<int>(nullable: true),
                    reward_maximagination = table.Column<int>(nullable: true),
                    reward_maxhealth = table.Column<int>(nullable: true),
                    reward_maxinventory = table.Column<int>(nullable: true),
                    reward_maxmodel = table.Column<int>(nullable: true),
                    reward_maxwidget = table.Column<int>(nullable: true),
                    reward_maxwallet = table.Column<long>(nullable: true),
                    repeatable = table.Column<bool>(nullable: true),
                    reward_currency_repeatable = table.Column<long>(nullable: true),
                    reward_item1_repeatable = table.Column<int>(nullable: true),
                    reward_item1_repeat_count = table.Column<int>(nullable: true),
                    reward_item2_repeatable = table.Column<int>(nullable: true),
                    reward_item2_repeat_count = table.Column<int>(nullable: true),
                    reward_item3_repeatable = table.Column<int>(nullable: true),
                    reward_item3_repeat_count = table.Column<int>(nullable: true),
                    reward_item4_repeatable = table.Column<int>(nullable: true),
                    reward_item4_repeat_count = table.Column<int>(nullable: true),
                    time_limit = table.Column<int>(nullable: true),
                    isMission = table.Column<bool>(nullable: true),
                    missionIconID = table.Column<int>(nullable: true),
                    prereqMissionID = table.Column<string>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    inMOTD = table.Column<bool>(nullable: true),
                    cooldownTime = table.Column<long>(nullable: true),
                    isRandom = table.Column<bool>(nullable: true),
                    randomPool = table.Column<string>(nullable: true),
                    UIPrereqID = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true),
                    HUDStates = table.Column<string>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    reward_bankinventory = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MissionTasks",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    taskType = table.Column<int>(nullable: true),
                    target = table.Column<int>(nullable: true),
                    targetGroup = table.Column<string>(nullable: true),
                    targetValue = table.Column<int>(nullable: true),
                    taskParam1 = table.Column<string>(nullable: true),
                    largeTaskIcon = table.Column<string>(nullable: true),
                    IconID = table.Column<int>(nullable: true),
                    uid = table.Column<int>(nullable: true),
                    largeTaskIconID = table.Column<int>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionTasks", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MissionText",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    story_icon = table.Column<string>(nullable: true),
                    missionIcon = table.Column<string>(nullable: true),
                    offerNPCIcon = table.Column<string>(nullable: true),
                    IconID = table.Column<int>(nullable: true),
                    state_1_anim = table.Column<string>(nullable: true),
                    state_2_anim = table.Column<string>(nullable: true),
                    state_3_anim = table.Column<string>(nullable: true),
                    state_4_anim = table.Column<string>(nullable: true),
                    state_3_turnin_anim = table.Column<string>(nullable: true),
                    state_4_turnin_anim = table.Column<string>(nullable: true),
                    onclick_anim = table.Column<string>(nullable: true),
                    CinematicAccepted = table.Column<string>(nullable: true),
                    CinematicAcceptedLeadin = table.Column<float>(nullable: true),
                    CinematicCompleted = table.Column<string>(nullable: true),
                    CinematicCompletedLeadin = table.Column<float>(nullable: true),
                    CinematicRepeatable = table.Column<string>(nullable: true),
                    CinematicRepeatableLeadin = table.Column<float>(nullable: true),
                    CinematicRepeatableCompleted = table.Column<string>(nullable: true),
                    CinematicRepeatableCompletedLeadin = table.Column<float>(nullable: true),
                    AudioEventGUID_Interact = table.Column<string>(nullable: true),
                    AudioEventGUID_OfferAccept = table.Column<string>(nullable: true),
                    AudioEventGUID_OfferDeny = table.Column<string>(nullable: true),
                    AudioEventGUID_Completed = table.Column<string>(nullable: true),
                    AudioEventGUID_TurnIn = table.Column<string>(nullable: true),
                    AudioEventGUID_Failed = table.Column<string>(nullable: true),
                    AudioEventGUID_Progress = table.Column<string>(nullable: true),
                    AudioMusicCue_OfferAccept = table.Column<string>(nullable: true),
                    AudioMusicCue_TurnIn = table.Column<string>(nullable: true),
                    turnInIconID = table.Column<int>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionText", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ModelBehavior",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    definitionXMLfilename = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelBehavior", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ModularBuildComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    buildType = table.Column<int>(nullable: true),
                    xml = table.Column<string>(nullable: true),
                    createdLOT = table.Column<int>(nullable: true),
                    createdPhysicsID = table.Column<int>(nullable: true),
                    AudioEventGUID_Snap = table.Column<string>(nullable: true),
                    AudioEventGUID_Complete = table.Column<string>(nullable: true),
                    AudioEventGUID_Present = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModularBuildComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ModuleComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    partCode = table.Column<int>(nullable: true),
                    buildType = table.Column<int>(nullable: true),
                    xml = table.Column<string>(nullable: true),
                    primarySoundGUID = table.Column<string>(nullable: true),
                    assembledEffectID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MotionFX",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    typeID = table.Column<int>(nullable: true),
                    slamVelocity = table.Column<float>(nullable: true),
                    addVelocity = table.Column<float>(nullable: true),
                    duration = table.Column<float>(nullable: true),
                    destGroupName = table.Column<string>(nullable: true),
                    startScale = table.Column<float>(nullable: true),
                    endScale = table.Column<float>(nullable: true),
                    velocity = table.Column<float>(nullable: true),
                    distance = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotionFX", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MovementAIComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    MovementType = table.Column<string>(nullable: true),
                    WanderChance = table.Column<float>(nullable: true),
                    WanderDelayMin = table.Column<float>(nullable: true),
                    WanderDelayMax = table.Column<float>(nullable: true),
                    WanderSpeed = table.Column<float>(nullable: true),
                    WanderRadius = table.Column<float>(nullable: true),
                    attachedPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementAIComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "MovingPlatforms",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    platformIsSimpleMover = table.Column<bool>(nullable: true),
                    platformMoveX = table.Column<float>(nullable: true),
                    platformMoveY = table.Column<float>(nullable: true),
                    platformMoveZ = table.Column<float>(nullable: true),
                    platformMoveTime = table.Column<float>(nullable: true),
                    platformStartAtEnd = table.Column<bool>(nullable: true),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovingPlatforms", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "NpcIcons",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    color = table.Column<int>(nullable: true),
                    offset = table.Column<float>(nullable: true),
                    LOT = table.Column<int>(nullable: true),
                    Texture = table.Column<string>(nullable: true),
                    isClickable = table.Column<bool>(nullable: true),
                    scale = table.Column<float>(nullable: true),
                    rotateToFace = table.Column<bool>(nullable: true),
                    compositeHorizOffset = table.Column<float>(nullable: true),
                    compositeVertOffset = table.Column<float>(nullable: true),
                    compositeScale = table.Column<float>(nullable: true),
                    compositeConnectionNode = table.Column<string>(nullable: true),
                    compositeLOTMultiMission = table.Column<int>(nullable: true),
                    compositeLOTMultiMissionVentor = table.Column<int>(nullable: true),
                    compositeIconTexture = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NpcIcons", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ObjectBehaviors",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BehaviorID = table.Column<long>(nullable: true),
                    xmldata = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectBehaviors", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ObjectBehaviorXREF",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LOT = table.Column<int>(nullable: true),
                    behaviorID1 = table.Column<long>(nullable: true),
                    behaviorID2 = table.Column<long>(nullable: true),
                    behaviorID3 = table.Column<long>(nullable: true),
                    behaviorID4 = table.Column<long>(nullable: true),
                    behaviorID5 = table.Column<long>(nullable: true),
                    type = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectBehaviorXREF", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Objects",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    placeable = table.Column<bool>(nullable: true),
                    type = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    npcTemplateID = table.Column<int>(nullable: true),
                    displayName = table.Column<string>(nullable: true),
                    interactionDistance = table.Column<float>(nullable: true),
                    nametag = table.Column<bool>(nullable: true),
                    _internalNotes = table.Column<string>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true),
                    HQ_valid = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objects", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ObjectSkills",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    objectTemplate = table.Column<int>(nullable: true),
                    skillID = table.Column<int>(nullable: true),
                    castOnType = table.Column<int>(nullable: true),
                    AICombatWeight = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectSkills", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "PackageComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    LootMatrixIndex = table.Column<int>(nullable: true),
                    packageType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "PetAbilities",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    AbilityName = table.Column<string>(nullable: true),
                    ImaginationCost = table.Column<int>(nullable: true),
                    locStatus = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetAbilities", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "PetComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    minTameUpdateTime = table.Column<float>(nullable: true),
                    maxTameUpdateTime = table.Column<float>(nullable: true),
                    percentTameChance = table.Column<float>(nullable: true),
                    tamability = table.Column<float>(nullable: true),
                    elementType = table.Column<int>(nullable: true),
                    walkSpeed = table.Column<float>(nullable: true),
                    runSpeed = table.Column<float>(nullable: true),
                    sprintSpeed = table.Column<float>(nullable: true),
                    idleTimeMin = table.Column<float>(nullable: true),
                    idleTimeMax = table.Column<float>(nullable: true),
                    petForm = table.Column<int>(nullable: true),
                    imaginationDrainRate = table.Column<float>(nullable: true),
                    AudioMetaEventSet = table.Column<string>(nullable: true),
                    buffIDs = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "PetNestComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    ElementalType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetNestComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "PhysicsComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    @static = table.Column<float>(name: "static", nullable: true),
                    physics_asset = table.Column<string>(nullable: true),
                    jump = table.Column<float>(nullable: true),
                    doublejump = table.Column<float>(nullable: true),
                    speed = table.Column<float>(nullable: true),
                    rotSpeed = table.Column<float>(nullable: true),
                    playerHeight = table.Column<float>(nullable: true),
                    playerRadius = table.Column<float>(nullable: true),
                    pcShapeType = table.Column<int>(nullable: true),
                    collisionGroup = table.Column<int>(nullable: true),
                    airSpeed = table.Column<float>(nullable: true),
                    boundaryAsset = table.Column<string>(nullable: true),
                    jumpAirSpeed = table.Column<float>(nullable: true),
                    friction = table.Column<float>(nullable: true),
                    gravityVolumeAsset = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicsComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerFlags",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    SessionOnly = table.Column<bool>(nullable: true),
                    OnlySetByServer = table.Column<bool>(nullable: true),
                    SessionZoneOnly = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFlags", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStatistics",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    statID = table.Column<int>(nullable: true),
                    sortOrder = table.Column<int>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatistics", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "PossessableComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    controlSchemeID = table.Column<int>(nullable: true),
                    minifigAttachPoint = table.Column<string>(nullable: true),
                    minifigAttachAnimation = table.Column<string>(nullable: true),
                    minifigDetachAnimation = table.Column<string>(nullable: true),
                    mountAttachAnimation = table.Column<string>(nullable: true),
                    mountDetachAnimation = table.Column<string>(nullable: true),
                    attachOffsetFwd = table.Column<float>(nullable: true),
                    attachOffsetRight = table.Column<float>(nullable: true),
                    possessionType = table.Column<int>(nullable: true),
                    wantBillboard = table.Column<bool>(nullable: true),
                    billboardOffsetUp = table.Column<float>(nullable: true),
                    depossessOnHit = table.Column<bool>(nullable: true),
                    hitStunTime = table.Column<float>(nullable: true),
                    skillSet = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PossessableComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Preconditions",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    type = table.Column<int>(nullable: true),
                    targetLOT = table.Column<string>(nullable: true),
                    targetGroup = table.Column<string>(nullable: true),
                    targetCount = table.Column<int>(nullable: true),
                    iconID = table.Column<int>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    validContexts = table.Column<long>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preconditions", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "PropertyEntranceComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    mapID = table.Column<int>(nullable: true),
                    propertyName = table.Column<string>(nullable: true),
                    isOnProperty = table.Column<bool>(nullable: true),
                    groupType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyEntranceComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "PropertyTemplate",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    mapID = table.Column<int>(nullable: true),
                    vendorMapID = table.Column<int>(nullable: true),
                    spawnName = table.Column<string>(nullable: true),
                    type = table.Column<int>(nullable: true),
                    sizecode = table.Column<int>(nullable: true),
                    minimumPrice = table.Column<int>(nullable: true),
                    rentDuration = table.Column<int>(nullable: true),
                    path = table.Column<string>(nullable: true),
                    cloneLimit = table.Column<int>(nullable: true),
                    durationType = table.Column<int>(nullable: true),
                    achievementRequired = table.Column<int>(nullable: true),
                    zoneX = table.Column<float>(nullable: true),
                    zoneY = table.Column<float>(nullable: true),
                    zoneZ = table.Column<float>(nullable: true),
                    maxBuildHeight = table.Column<float>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    reputationPerMinute = table.Column<int>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTemplate", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ProximityMonitorComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    Proximities = table.Column<string>(nullable: true),
                    LoadOnClient = table.Column<bool>(nullable: true),
                    LoadOnServer = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProximityMonitorComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ProximityTypes",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Radius = table.Column<int>(nullable: true),
                    CollisionGroup = table.Column<int>(nullable: true),
                    PassiveChecks = table.Column<bool>(nullable: true),
                    IconID = table.Column<int>(nullable: true),
                    LoadOnClient = table.Column<bool>(nullable: true),
                    LoadOnServer = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProximityTypes", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RacingModuleComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    topSpeed = table.Column<float>(nullable: true),
                    acceleration = table.Column<float>(nullable: true),
                    handling = table.Column<float>(nullable: true),
                    stability = table.Column<float>(nullable: true),
                    imagination = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RacingModuleComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RailActivatorComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    startAnim = table.Column<string>(nullable: true),
                    loopAnim = table.Column<string>(nullable: true),
                    stopAnim = table.Column<string>(nullable: true),
                    startSound = table.Column<string>(nullable: true),
                    loopSound = table.Column<string>(nullable: true),
                    stopSound = table.Column<string>(nullable: true),
                    effectIDs = table.Column<string>(nullable: true),
                    preconditions = table.Column<string>(nullable: true),
                    playerCollision = table.Column<bool>(nullable: true),
                    cameraLocked = table.Column<bool>(nullable: true),
                    StartEffectID = table.Column<string>(nullable: true),
                    StopEffectID = table.Column<string>(nullable: true),
                    DamageImmune = table.Column<bool>(nullable: true),
                    NoAggro = table.Column<bool>(nullable: true),
                    ShowNameBillboard = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RailActivatorComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RarityTable",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    randmax = table.Column<float>(nullable: true),
                    rarity = table.Column<int>(nullable: true),
                    RarityTableIndex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RarityTable", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RarityTableIndex",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RarityTableIndex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RarityTableIndex", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RebuildComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    reset_time = table.Column<float>(nullable: true),
                    complete_time = table.Column<float>(nullable: true),
                    take_imagination = table.Column<int>(nullable: true),
                    interruptible = table.Column<bool>(nullable: true),
                    self_activator = table.Column<bool>(nullable: true),
                    custom_modules = table.Column<string>(nullable: true),
                    activityID = table.Column<int>(nullable: true),
                    post_imagination_cost = table.Column<int>(nullable: true),
                    time_before_smash = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RebuildComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RebuildSections",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    rebuildID = table.Column<int>(nullable: true),
                    objectID = table.Column<int>(nullable: true),
                    offset_x = table.Column<float>(nullable: true),
                    offset_y = table.Column<float>(nullable: true),
                    offset_z = table.Column<float>(nullable: true),
                    fall_angle_x = table.Column<float>(nullable: true),
                    fall_angle_y = table.Column<float>(nullable: true),
                    fall_angle_z = table.Column<float>(nullable: true),
                    fall_height = table.Column<float>(nullable: true),
                    requires_list = table.Column<string>(nullable: true),
                    size = table.Column<int>(nullable: true),
                    bPlaced = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RebuildSections", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Release_Version",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReleaseVersion = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Release_Version", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RenderComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    render_asset = table.Column<string>(nullable: true),
                    icon_asset = table.Column<string>(nullable: true),
                    IconID = table.Column<int>(nullable: true),
                    shader_id = table.Column<int>(nullable: true),
                    effect1 = table.Column<int>(nullable: true),
                    effect2 = table.Column<int>(nullable: true),
                    effect3 = table.Column<int>(nullable: true),
                    effect4 = table.Column<int>(nullable: true),
                    effect5 = table.Column<int>(nullable: true),
                    effect6 = table.Column<int>(nullable: true),
                    animationGroupIDs = table.Column<string>(nullable: true),
                    fade = table.Column<bool>(nullable: true),
                    usedropshadow = table.Column<bool>(nullable: true),
                    preloadAnimations = table.Column<bool>(nullable: true),
                    fadeInTime = table.Column<float>(nullable: true),
                    maxShadowDistance = table.Column<float>(nullable: true),
                    ignoreCameraCollision = table.Column<bool>(nullable: true),
                    renderComponentLOD1 = table.Column<int>(nullable: true),
                    renderComponentLOD2 = table.Column<int>(nullable: true),
                    gradualSnap = table.Column<bool>(nullable: true),
                    animationFlag = table.Column<int>(nullable: true),
                    AudioMetaEventSet = table.Column<string>(nullable: true),
                    billboardHeight = table.Column<float>(nullable: true),
                    chatBubbleOffset = table.Column<float>(nullable: true),
                    staticBillboard = table.Column<bool>(nullable: true),
                    LXFMLFolder = table.Column<string>(nullable: true),
                    attachIndicatorsToNode = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenderComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RenderComponentFlash",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    interactive = table.Column<bool>(nullable: true),
                    animated = table.Column<bool>(nullable: true),
                    nodeName = table.Column<string>(nullable: true),
                    flashPath = table.Column<string>(nullable: true),
                    elementName = table.Column<string>(nullable: true),
                    _uid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenderComponentFlash", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RenderComponentWrapper",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    defaultWrapperAsset = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenderComponentWrapper", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RenderIconAssets",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    icon_asset = table.Column<string>(nullable: true),
                    blank_column = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenderIconAssets", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ReputationRewards",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    repLevel = table.Column<int>(nullable: true),
                    sublevel = table.Column<int>(nullable: true),
                    reputation = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReputationRewards", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RewardCodes",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    code = table.Column<string>(nullable: true),
                    attachmentLOT = table.Column<int>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardCodes", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    LevelID = table.Column<int>(nullable: true),
                    MissionID = table.Column<int>(nullable: true),
                    RewardType = table.Column<int>(nullable: true),
                    value = table.Column<int>(nullable: true),
                    count = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "RocketLaunchpadControlComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    targetZone = table.Column<int>(nullable: true),
                    defaultZoneID = table.Column<int>(nullable: true),
                    targetScene = table.Column<string>(nullable: true),
                    gmLevel = table.Column<int>(nullable: true),
                    playerAnimation = table.Column<string>(nullable: true),
                    rocketAnimation = table.Column<string>(nullable: true),
                    launchMusic = table.Column<string>(nullable: true),
                    useLaunchPrecondition = table.Column<bool>(nullable: true),
                    useAltLandingPrecondition = table.Column<bool>(nullable: true),
                    launchPrecondition = table.Column<string>(nullable: true),
                    altLandingPrecondition = table.Column<string>(nullable: true),
                    altLandingSpawnPointName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RocketLaunchpadControlComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "SceneTable",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    sceneID = table.Column<int>(nullable: true),
                    sceneName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneTable", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ScriptComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    script_name = table.Column<string>(nullable: true),
                    client_script_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "SkillBehavior",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    skillID = table.Column<int>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    behaviorID = table.Column<int>(nullable: true),
                    imaginationcost = table.Column<int>(nullable: true),
                    cooldowngroup = table.Column<int>(nullable: true),
                    cooldown = table.Column<float>(nullable: true),
                    inNpcEditor = table.Column<bool>(nullable: true),
                    skillIcon = table.Column<int>(nullable: true),
                    oomSkillID = table.Column<string>(nullable: true),
                    oomBehaviorEffectID = table.Column<int>(nullable: true),
                    castTypeDesc = table.Column<int>(nullable: true),
                    imBonusUI = table.Column<int>(nullable: true),
                    lifeBonusUI = table.Column<int>(nullable: true),
                    armorBonusUI = table.Column<int>(nullable: true),
                    damageUI = table.Column<int>(nullable: true),
                    hideIcon = table.Column<bool>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    gate_version = table.Column<string>(nullable: true),
                    cancelType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillBehavior", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "SmashableChain",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    chainIndex = table.Column<int>(nullable: true),
                    chainLevel = table.Column<int>(nullable: true),
                    lootMatrixID = table.Column<int>(nullable: true),
                    rarityTableIndex = table.Column<int>(nullable: true),
                    currencyIndex = table.Column<int>(nullable: true),
                    currencyLevel = table.Column<int>(nullable: true),
                    smashCount = table.Column<int>(nullable: true),
                    timeLimit = table.Column<int>(nullable: true),
                    chainStepID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmashableChain", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "SmashableChainIndex",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    targetGroup = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    continuous = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmashableChainIndex", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "SmashableComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    LootMatrixIndex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmashableComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "SmashableElements",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    elementID = table.Column<int>(nullable: true),
                    dropWeight = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmashableElements", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "SpeedchatMenu",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    parentId = table.Column<int>(nullable: true),
                    emoteId = table.Column<int>(nullable: true),
                    imageName = table.Column<string>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeedchatMenu", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPricing",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    countryCode = table.Column<string>(nullable: true),
                    monthlyFeeGold = table.Column<string>(nullable: true),
                    monthlyFeeSilver = table.Column<string>(nullable: true),
                    monthlyFeeBronze = table.Column<string>(nullable: true),
                    monetarySymbol = table.Column<int>(nullable: true),
                    symbolIsAppended = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPricing", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "SurfaceType",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurfaceType = table.Column<int>(nullable: true),
                    FootstepNDAudioMetaEventSetName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurfaceType", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "sysdiagrams",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(nullable: true),
                    principal_id = table.Column<int>(nullable: true),
                    diagram_id = table.Column<int>(nullable: true),
                    version = table.Column<int>(nullable: true),
                    definition = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sysdiagrams", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "TamingBuildPuzzles",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    PuzzleModelLot = table.Column<int>(nullable: true),
                    NPCLot = table.Column<int>(nullable: true),
                    ValidPiecesLXF = table.Column<string>(nullable: true),
                    InvalidPiecesLXF = table.Column<string>(nullable: true),
                    Difficulty = table.Column<int>(nullable: true),
                    Timelimit = table.Column<int>(nullable: true),
                    NumValidPieces = table.Column<int>(nullable: true),
                    TotalNumPieces = table.Column<int>(nullable: true),
                    ModelName = table.Column<string>(nullable: true),
                    FullModelLXF = table.Column<string>(nullable: true),
                    Duration = table.Column<float>(nullable: true),
                    imagCostPerBuild = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TamingBuildPuzzles", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "TextDescription",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TextID = table.Column<int>(nullable: true),
                    TestDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextDescription", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "TextLanguage",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TextID = table.Column<int>(nullable: true),
                    LanguageID = table.Column<int>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextLanguage", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "TrailEffects",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    trailID = table.Column<int>(nullable: true),
                    textureName = table.Column<string>(nullable: true),
                    blendmode = table.Column<int>(nullable: true),
                    cardlifetime = table.Column<float>(nullable: true),
                    colorlifetime = table.Column<float>(nullable: true),
                    minTailFade = table.Column<float>(nullable: true),
                    tailFade = table.Column<float>(nullable: true),
                    max_particles = table.Column<int>(nullable: true),
                    birthDelay = table.Column<float>(nullable: true),
                    deathDelay = table.Column<float>(nullable: true),
                    bone1 = table.Column<string>(nullable: true),
                    bone2 = table.Column<string>(nullable: true),
                    texLength = table.Column<float>(nullable: true),
                    texWidth = table.Column<float>(nullable: true),
                    startColorR = table.Column<float>(nullable: true),
                    startColorG = table.Column<float>(nullable: true),
                    startColorB = table.Column<float>(nullable: true),
                    startColorA = table.Column<float>(nullable: true),
                    middleColorR = table.Column<float>(nullable: true),
                    middleColorG = table.Column<float>(nullable: true),
                    middleColorB = table.Column<float>(nullable: true),
                    middleColorA = table.Column<float>(nullable: true),
                    endColorR = table.Column<float>(nullable: true),
                    endColorG = table.Column<float>(nullable: true),
                    endColorB = table.Column<float>(nullable: true),
                    endColorA = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrailEffects", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "UGBehaviorSounds",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    guid = table.Column<string>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    gate_version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UGBehaviorSounds", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "VehiclePhysics",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    hkxFilename = table.Column<string>(nullable: true),
                    fGravityScale = table.Column<float>(nullable: true),
                    fMass = table.Column<float>(nullable: true),
                    fChassisFriction = table.Column<float>(nullable: true),
                    fMaxSpeed = table.Column<float>(nullable: true),
                    fEngineTorque = table.Column<float>(nullable: true),
                    fBrakeFrontTorque = table.Column<float>(nullable: true),
                    fBrakeRearTorque = table.Column<float>(nullable: true),
                    fBrakeMinInputToBlock = table.Column<float>(nullable: true),
                    fBrakeMinTimeToBlock = table.Column<float>(nullable: true),
                    fSteeringMaxAngle = table.Column<float>(nullable: true),
                    fSteeringSpeedLimitForMaxAngle = table.Column<float>(nullable: true),
                    fSteeringMinAngle = table.Column<float>(nullable: true),
                    fFwdBias = table.Column<float>(nullable: true),
                    fFrontTireFriction = table.Column<float>(nullable: true),
                    fRearTireFriction = table.Column<float>(nullable: true),
                    fFrontTireFrictionSlide = table.Column<float>(nullable: true),
                    fRearTireFrictionSlide = table.Column<float>(nullable: true),
                    fFrontTireSlipAngle = table.Column<float>(nullable: true),
                    fRearTireSlipAngle = table.Column<float>(nullable: true),
                    fWheelWidth = table.Column<float>(nullable: true),
                    fWheelRadius = table.Column<float>(nullable: true),
                    fWheelMass = table.Column<float>(nullable: true),
                    fReorientPitchStrength = table.Column<float>(nullable: true),
                    fReorientRollStrength = table.Column<float>(nullable: true),
                    fSuspensionLength = table.Column<float>(nullable: true),
                    fSuspensionStrength = table.Column<float>(nullable: true),
                    fSuspensionDampingCompression = table.Column<float>(nullable: true),
                    fSuspensionDampingRelaxation = table.Column<float>(nullable: true),
                    iChassisCollisionGroup = table.Column<int>(nullable: true),
                    fNormalSpinDamping = table.Column<float>(nullable: true),
                    fCollisionSpinDamping = table.Column<float>(nullable: true),
                    fCollisionThreshold = table.Column<float>(nullable: true),
                    fTorqueRollFactor = table.Column<float>(nullable: true),
                    fTorquePitchFactor = table.Column<float>(nullable: true),
                    fTorqueYawFactor = table.Column<float>(nullable: true),
                    fInertiaRoll = table.Column<float>(nullable: true),
                    fInertiaPitch = table.Column<float>(nullable: true),
                    fInertiaYaw = table.Column<float>(nullable: true),
                    fExtraTorqueFactor = table.Column<float>(nullable: true),
                    fCenterOfMassFwd = table.Column<float>(nullable: true),
                    fCenterOfMassUp = table.Column<float>(nullable: true),
                    fCenterOfMassRight = table.Column<float>(nullable: true),
                    fWheelHardpointFrontFwd = table.Column<float>(nullable: true),
                    fWheelHardpointFrontUp = table.Column<float>(nullable: true),
                    fWheelHardpointFrontRight = table.Column<float>(nullable: true),
                    fWheelHardpointRearFwd = table.Column<float>(nullable: true),
                    fWheelHardpointRearUp = table.Column<float>(nullable: true),
                    fWheelHardpointRearRight = table.Column<float>(nullable: true),
                    fInputTurnSpeed = table.Column<float>(nullable: true),
                    fInputDeadTurnBackSpeed = table.Column<float>(nullable: true),
                    fInputAccelSpeed = table.Column<float>(nullable: true),
                    fInputDeadAccelDownSpeed = table.Column<float>(nullable: true),
                    fInputDecelSpeed = table.Column<float>(nullable: true),
                    fInputDeadDecelDownSpeed = table.Column<float>(nullable: true),
                    fInputSlopeChangePointX = table.Column<float>(nullable: true),
                    fInputInitialSlope = table.Column<float>(nullable: true),
                    fInputDeadZone = table.Column<float>(nullable: true),
                    fAeroAirDensity = table.Column<float>(nullable: true),
                    fAeroFrontalArea = table.Column<float>(nullable: true),
                    fAeroDragCoefficient = table.Column<float>(nullable: true),
                    fAeroLiftCoefficient = table.Column<float>(nullable: true),
                    fAeroExtraGravity = table.Column<float>(nullable: true),
                    fBoostTopSpeed = table.Column<float>(nullable: true),
                    fBoostCostPerSecond = table.Column<float>(nullable: true),
                    fBoostAccelerateChange = table.Column<float>(nullable: true),
                    fBoostDampingChange = table.Column<float>(nullable: true),
                    fPowerslideNeutralAngle = table.Column<float>(nullable: true),
                    fPowerslideTorqueStrength = table.Column<float>(nullable: true),
                    iPowerslideNumTorqueApplications = table.Column<int>(nullable: true),
                    fImaginationTankSize = table.Column<float>(nullable: true),
                    fSkillCost = table.Column<float>(nullable: true),
                    fWreckSpeedBase = table.Column<float>(nullable: true),
                    fWreckSpeedPercent = table.Column<float>(nullable: true),
                    fWreckMinAngle = table.Column<float>(nullable: true),
                    AudioEventEngine = table.Column<string>(nullable: true),
                    AudioEventSkid = table.Column<string>(nullable: true),
                    AudioEventLightHit = table.Column<string>(nullable: true),
                    AudioSpeedThresholdLightHit = table.Column<float>(nullable: true),
                    AudioTimeoutLightHit = table.Column<float>(nullable: true),
                    AudioEventHeavyHit = table.Column<string>(nullable: true),
                    AudioSpeedThresholdHeavyHit = table.Column<float>(nullable: true),
                    AudioTimeoutHeavyHit = table.Column<float>(nullable: true),
                    AudioEventStart = table.Column<string>(nullable: true),
                    AudioEventTreadConcrete = table.Column<string>(nullable: true),
                    AudioEventTreadSand = table.Column<string>(nullable: true),
                    AudioEventTreadWood = table.Column<string>(nullable: true),
                    AudioEventTreadDirt = table.Column<string>(nullable: true),
                    AudioEventTreadPlastic = table.Column<string>(nullable: true),
                    AudioEventTreadGrass = table.Column<string>(nullable: true),
                    AudioEventTreadGravel = table.Column<string>(nullable: true),
                    AudioEventTreadMud = table.Column<string>(nullable: true),
                    AudioEventTreadWater = table.Column<string>(nullable: true),
                    AudioEventTreadSnow = table.Column<string>(nullable: true),
                    AudioEventTreadIce = table.Column<string>(nullable: true),
                    AudioEventTreadMetal = table.Column<string>(nullable: true),
                    AudioEventTreadLeaves = table.Column<string>(nullable: true),
                    AudioEventLightLand = table.Column<string>(nullable: true),
                    AudioAirtimeForLightLand = table.Column<float>(nullable: true),
                    AudioEventHeavyLand = table.Column<string>(nullable: true),
                    AudioAirtimeForHeavyLand = table.Column<float>(nullable: true),
                    bWheelsVisible = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclePhysics", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "VehicleStatMap",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    ModuleStat = table.Column<string>(nullable: true),
                    HavokStat = table.Column<string>(nullable: true),
                    HavokChangePerModuleStat = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleStatMap", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "VendorComponent",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    buyScalar = table.Column<float>(nullable: true),
                    sellScalar = table.Column<float>(nullable: true),
                    refreshTimeSeconds = table.Column<float>(nullable: true),
                    LootMatrixIndex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorComponent", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "WhatsCoolItemSpotlight",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    itemID = table.Column<int>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    gate_version = table.Column<string>(nullable: true),
                    locStatus = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsCoolItemSpotlight", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "WhatsCoolNewsAndTips",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    iconID = table.Column<int>(nullable: true),
                    type = table.Column<int>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    gate_version = table.Column<string>(nullable: true),
                    locStatus = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsCoolNewsAndTips", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "WorldConfig",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorldConfigID = table.Column<int>(nullable: true),
                    pegravityvalue = table.Column<float>(nullable: true),
                    pebroadphaseworldsize = table.Column<float>(nullable: true),
                    pegameobjscalefactor = table.Column<float>(nullable: true),
                    character_rotation_speed = table.Column<float>(nullable: true),
                    character_walk_forward_speed = table.Column<float>(nullable: true),
                    character_walk_backward_speed = table.Column<float>(nullable: true),
                    character_walk_strafe_speed = table.Column<float>(nullable: true),
                    character_walk_strafe_forward_speed = table.Column<float>(nullable: true),
                    character_walk_strafe_backward_speed = table.Column<float>(nullable: true),
                    character_run_backward_speed = table.Column<float>(nullable: true),
                    character_run_strafe_speed = table.Column<float>(nullable: true),
                    character_run_strafe_forward_speed = table.Column<float>(nullable: true),
                    character_run_strafe_backward_speed = table.Column<float>(nullable: true),
                    global_cooldown = table.Column<float>(nullable: true),
                    characterGroundedTime = table.Column<float>(nullable: true),
                    characterGroundedSpeed = table.Column<float>(nullable: true),
                    globalImmunityTime = table.Column<float>(nullable: true),
                    character_max_slope = table.Column<float>(nullable: true),
                    defaultrespawntime = table.Column<float>(nullable: true),
                    mission_tooltip_timeout = table.Column<float>(nullable: true),
                    vendor_buy_multiplier = table.Column<float>(nullable: true),
                    pet_follow_radius = table.Column<float>(nullable: true),
                    character_eye_height = table.Column<float>(nullable: true),
                    flight_vertical_velocity = table.Column<float>(nullable: true),
                    flight_airspeed = table.Column<float>(nullable: true),
                    flight_fuel_ratio = table.Column<float>(nullable: true),
                    flight_max_airspeed = table.Column<float>(nullable: true),
                    fReputationPerVote = table.Column<float>(nullable: true),
                    nPropertyCloneLimit = table.Column<int>(nullable: true),
                    defaultHomespaceTemplate = table.Column<int>(nullable: true),
                    coins_lost_on_death_percent = table.Column<float>(nullable: true),
                    coins_lost_on_death_min = table.Column<int>(nullable: true),
                    coins_lost_on_death_max = table.Column<int>(nullable: true),
                    character_votes_per_day = table.Column<int>(nullable: true),
                    property_moderation_request_approval_cost = table.Column<int>(nullable: true),
                    property_moderation_request_review_cost = table.Column<int>(nullable: true),
                    propertyModRequestsAllowedSpike = table.Column<int>(nullable: true),
                    propertyModRequestsAllowedInterval = table.Column<int>(nullable: true),
                    propertyModRequestsAllowedTotal = table.Column<int>(nullable: true),
                    propertyModRequestsSpikeDuration = table.Column<int>(nullable: true),
                    propertyModRequestsIntervalDuration = table.Column<int>(nullable: true),
                    modelModerateOnCreate = table.Column<bool>(nullable: true),
                    defaultPropertyMaxHeight = table.Column<float>(nullable: true),
                    reputationPerVoteCast = table.Column<float>(nullable: true),
                    reputationPerVoteReceived = table.Column<float>(nullable: true),
                    showcaseTopModelConsiderationBattles = table.Column<int>(nullable: true),
                    reputationPerBattlePromotion = table.Column<float>(nullable: true),
                    coins_lost_on_death_min_timeout = table.Column<float>(nullable: true),
                    coins_lost_on_death_max_timeout = table.Column<float>(nullable: true),
                    mail_base_fee = table.Column<int>(nullable: true),
                    mail_percent_attachment_fee = table.Column<float>(nullable: true),
                    propertyReputationDelay = table.Column<int>(nullable: true),
                    LevelCap = table.Column<int>(nullable: true),
                    LevelUpBehaviorEffect = table.Column<string>(nullable: true),
                    CharacterVersion = table.Column<int>(nullable: true),
                    LevelCapCurrencyConversion = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldConfig", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ZoneLoadingTips",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<int>(nullable: true),
                    zoneid = table.Column<int>(nullable: true),
                    imagelocation = table.Column<string>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    gate_version = table.Column<string>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    weight = table.Column<int>(nullable: true),
                    targetVersion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneLoadingTips", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ZoneSummary",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    zoneID = table.Column<int>(nullable: true),
                    type = table.Column<int>(nullable: true),
                    value = table.Column<int>(nullable: true),
                    _uniqueID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneSummary", x => x.efId);
                });

            migrationBuilder.CreateTable(
                name: "ZoneTable",
                columns: table => new
                {
                    efId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    zoneID = table.Column<int>(nullable: true),
                    locStatus = table.Column<int>(nullable: true),
                    zoneName = table.Column<string>(nullable: true),
                    scriptID = table.Column<int>(nullable: true),
                    ghostdistance_min = table.Column<float>(nullable: true),
                    ghostdistance = table.Column<float>(nullable: true),
                    population_soft_cap = table.Column<int>(nullable: true),
                    population_hard_cap = table.Column<int>(nullable: true),
                    DisplayDescription = table.Column<string>(nullable: true),
                    mapFolder = table.Column<string>(nullable: true),
                    smashableMinDistance = table.Column<float>(nullable: true),
                    smashableMaxDistance = table.Column<float>(nullable: true),
                    mixerProgram = table.Column<string>(nullable: true),
                    clientPhysicsFramerate = table.Column<string>(nullable: true),
                    serverPhysicsFramerate = table.Column<string>(nullable: true),
                    zoneControlTemplate = table.Column<int>(nullable: true),
                    widthInChunks = table.Column<int>(nullable: true),
                    heightInChunks = table.Column<int>(nullable: true),
                    petsAllowed = table.Column<bool>(nullable: true),
                    localize = table.Column<bool>(nullable: true),
                    fZoneWeight = table.Column<float>(nullable: true),
                    thumbnail = table.Column<string>(nullable: true),
                    PlayerLoseCoinsOnDeath = table.Column<bool>(nullable: true),
                    disableSaveLoc = table.Column<bool>(nullable: true),
                    teamRadius = table.Column<float>(nullable: true),
                    gate_version = table.Column<string>(nullable: true),
                    mountsAllowed = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneTable", x => x.efId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessoryDefaultLoc");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "ActivityRewards");

            migrationBuilder.DropTable(
                name: "ActivityText");

            migrationBuilder.DropTable(
                name: "AICombatRoles");

            migrationBuilder.DropTable(
                name: "AnimationIndex");

            migrationBuilder.DropTable(
                name: "Animations");

            migrationBuilder.DropTable(
                name: "BaseCombatAIComponent");

            migrationBuilder.DropTable(
                name: "BehaviorEffect");

            migrationBuilder.DropTable(
                name: "BehaviorParameter");

            migrationBuilder.DropTable(
                name: "BehaviorTemplate");

            migrationBuilder.DropTable(
                name: "BehaviorTemplateName");

            migrationBuilder.DropTable(
                name: "Blueprints");

            migrationBuilder.DropTable(
                name: "brickAttributes");

            migrationBuilder.DropTable(
                name: "BrickColors");

            migrationBuilder.DropTable(
                name: "BrickIDTable");

            migrationBuilder.DropTable(
                name: "BuffDefinitions");

            migrationBuilder.DropTable(
                name: "BuffParameters");

            migrationBuilder.DropTable(
                name: "Camera");

            migrationBuilder.DropTable(
                name: "CelebrationParameters");

            migrationBuilder.DropTable(
                name: "ChoiceBuildComponent");

            migrationBuilder.DropTable(
                name: "CollectibleComponent");

            migrationBuilder.DropTable(
                name: "ComponentsRegistry");

            migrationBuilder.DropTable(
                name: "ControlSchemes");

            migrationBuilder.DropTable(
                name: "CurrencyDenominations");

            migrationBuilder.DropTable(
                name: "CurrencyTable");

            migrationBuilder.DropTable(
                name: "DBExclude");

            migrationBuilder.DropTable(
                name: "DeletionRestrictions");

            migrationBuilder.DropTable(
                name: "DestructibleComponent");

            migrationBuilder.DropTable(
                name: "DevModelBehaviors");

            migrationBuilder.DropTable(
                name: "dtproperties");

            migrationBuilder.DropTable(
                name: "Emotes");

            migrationBuilder.DropTable(
                name: "EventGating");

            migrationBuilder.DropTable(
                name: "ExhibitComponent");

            migrationBuilder.DropTable(
                name: "Factions");

            migrationBuilder.DropTable(
                name: "FeatureGating");

            migrationBuilder.DropTable(
                name: "FlairTable");

            migrationBuilder.DropTable(
                name: "Icons");

            migrationBuilder.DropTable(
                name: "InventoryComponent");

            migrationBuilder.DropTable(
                name: "ItemComponent");

            migrationBuilder.DropTable(
                name: "ItemEggData");

            migrationBuilder.DropTable(
                name: "ItemFoodData");

            migrationBuilder.DropTable(
                name: "ItemSets");

            migrationBuilder.DropTable(
                name: "ItemSetSkills");

            migrationBuilder.DropTable(
                name: "JetPackPadComponent");

            migrationBuilder.DropTable(
                name: "LanguageType");

            migrationBuilder.DropTable(
                name: "LevelProgressionLookup");

            migrationBuilder.DropTable(
                name: "LootMatrix");

            migrationBuilder.DropTable(
                name: "LootMatrixIndex");

            migrationBuilder.DropTable(
                name: "LootTable");

            migrationBuilder.DropTable(
                name: "LootTableIndex");

            migrationBuilder.DropTable(
                name: "LUPExhibitComponent");

            migrationBuilder.DropTable(
                name: "LUPExhibitModelData");

            migrationBuilder.DropTable(
                name: "LUPZoneIDs");

            migrationBuilder.DropTable(
                name: "map_BlueprintCategory");

            migrationBuilder.DropTable(
                name: "mapAnimationPriorities");

            migrationBuilder.DropTable(
                name: "mapAssetType");

            migrationBuilder.DropTable(
                name: "mapIcon");

            migrationBuilder.DropTable(
                name: "mapItemTypes");

            migrationBuilder.DropTable(
                name: "mapRenderEffects");

            migrationBuilder.DropTable(
                name: "mapShaders");

            migrationBuilder.DropTable(
                name: "mapTextureResource");

            migrationBuilder.DropTable(
                name: "MinifigComponent");

            migrationBuilder.DropTable(
                name: "MinifigDecals_Eyebrows");

            migrationBuilder.DropTable(
                name: "MinifigDecals_Eyes");

            migrationBuilder.DropTable(
                name: "MinifigDecals_Legs");

            migrationBuilder.DropTable(
                name: "MinifigDecals_Mouths");

            migrationBuilder.DropTable(
                name: "MinifigDecals_Torsos");

            migrationBuilder.DropTable(
                name: "MissionEmail");

            migrationBuilder.DropTable(
                name: "MissionNPCComponent");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "MissionTasks");

            migrationBuilder.DropTable(
                name: "MissionText");

            migrationBuilder.DropTable(
                name: "ModelBehavior");

            migrationBuilder.DropTable(
                name: "ModularBuildComponent");

            migrationBuilder.DropTable(
                name: "ModuleComponent");

            migrationBuilder.DropTable(
                name: "MotionFX");

            migrationBuilder.DropTable(
                name: "MovementAIComponent");

            migrationBuilder.DropTable(
                name: "MovingPlatforms");

            migrationBuilder.DropTable(
                name: "NpcIcons");

            migrationBuilder.DropTable(
                name: "ObjectBehaviors");

            migrationBuilder.DropTable(
                name: "ObjectBehaviorXREF");

            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "ObjectSkills");

            migrationBuilder.DropTable(
                name: "PackageComponent");

            migrationBuilder.DropTable(
                name: "PetAbilities");

            migrationBuilder.DropTable(
                name: "PetComponent");

            migrationBuilder.DropTable(
                name: "PetNestComponent");

            migrationBuilder.DropTable(
                name: "PhysicsComponent");

            migrationBuilder.DropTable(
                name: "PlayerFlags");

            migrationBuilder.DropTable(
                name: "PlayerStatistics");

            migrationBuilder.DropTable(
                name: "PossessableComponent");

            migrationBuilder.DropTable(
                name: "Preconditions");

            migrationBuilder.DropTable(
                name: "PropertyEntranceComponent");

            migrationBuilder.DropTable(
                name: "PropertyTemplate");

            migrationBuilder.DropTable(
                name: "ProximityMonitorComponent");

            migrationBuilder.DropTable(
                name: "ProximityTypes");

            migrationBuilder.DropTable(
                name: "RacingModuleComponent");

            migrationBuilder.DropTable(
                name: "RailActivatorComponent");

            migrationBuilder.DropTable(
                name: "RarityTable");

            migrationBuilder.DropTable(
                name: "RarityTableIndex");

            migrationBuilder.DropTable(
                name: "RebuildComponent");

            migrationBuilder.DropTable(
                name: "RebuildSections");

            migrationBuilder.DropTable(
                name: "Release_Version");

            migrationBuilder.DropTable(
                name: "RenderComponent");

            migrationBuilder.DropTable(
                name: "RenderComponentFlash");

            migrationBuilder.DropTable(
                name: "RenderComponentWrapper");

            migrationBuilder.DropTable(
                name: "RenderIconAssets");

            migrationBuilder.DropTable(
                name: "ReputationRewards");

            migrationBuilder.DropTable(
                name: "RewardCodes");

            migrationBuilder.DropTable(
                name: "Rewards");

            migrationBuilder.DropTable(
                name: "RocketLaunchpadControlComponent");

            migrationBuilder.DropTable(
                name: "SceneTable");

            migrationBuilder.DropTable(
                name: "ScriptComponent");

            migrationBuilder.DropTable(
                name: "SkillBehavior");

            migrationBuilder.DropTable(
                name: "SmashableChain");

            migrationBuilder.DropTable(
                name: "SmashableChainIndex");

            migrationBuilder.DropTable(
                name: "SmashableComponent");

            migrationBuilder.DropTable(
                name: "SmashableElements");

            migrationBuilder.DropTable(
                name: "SpeedchatMenu");

            migrationBuilder.DropTable(
                name: "SubscriptionPricing");

            migrationBuilder.DropTable(
                name: "SurfaceType");

            migrationBuilder.DropTable(
                name: "sysdiagrams");

            migrationBuilder.DropTable(
                name: "TamingBuildPuzzles");

            migrationBuilder.DropTable(
                name: "TextDescription");

            migrationBuilder.DropTable(
                name: "TextLanguage");

            migrationBuilder.DropTable(
                name: "TrailEffects");

            migrationBuilder.DropTable(
                name: "UGBehaviorSounds");

            migrationBuilder.DropTable(
                name: "VehiclePhysics");

            migrationBuilder.DropTable(
                name: "VehicleStatMap");

            migrationBuilder.DropTable(
                name: "VendorComponent");

            migrationBuilder.DropTable(
                name: "WhatsCoolItemSpotlight");

            migrationBuilder.DropTable(
                name: "WhatsCoolNewsAndTips");

            migrationBuilder.DropTable(
                name: "WorldConfig");

            migrationBuilder.DropTable(
                name: "ZoneLoadingTips");

            migrationBuilder.DropTable(
                name: "ZoneSummary");

            migrationBuilder.DropTable(
                name: "ZoneTable");
        }
    }
}
