--------------------------------------------------------------
-- Constants/Settings for Zorillo Scripts
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------


CONSTANTS = {}
--------------------------------------------------------------
-- Other
--------------------------------------------------------------
CONSTANTS["NO_OBJECT"] = "0"
CONSTANTS["EMPTY_ID_NAME"] = "|" .. CONSTANTS["NO_OBJECT"]
CONSTANTS["LOT_NULL"] = -1


--------------------------------------------------------------
-- Skills
--------------------------------------------------------------
CONSTANTS["SKUNK_STINK_SKILL"] = 33
CONSTANTS["DESTINK_SKILL"] = 116
CONSTANTS["REMOVE_STINK_SKILL"] = 124


--------------------------------------------------------------
-- Special LOTs
--------------------------------------------------------------
CONSTANTS["SINGLE_LAMP_LOT"] = 3180				-- the LOT for the single lamp posts
CONSTANTS["SPOUT_LOT"] = 3283				    -- the LOT for the double lamp posts
CONSTANTS["LAMP_DETECTOR_LOT"] = 3434			-- the LOT for the skunk detectors that pop out of the lamp posts
CONSTANTS["EARTHQUAKE_CENTER_LOT"] = 3378		-- the LOT for the skunk statue in the center of town.  The earthquake radius goes out from here.
CONSTANTS["FOUNTAIN_ALERT_LOT"] = 3674		    -- the LOT for the fountain base with the status alert indicator
CONSTANTS["INVASION_STINK_CLOUD_LOT"] = 3851    -- the LOT for invasion stink clouds
CONSTANTS["INVENTOR_BUILDING_LOT"] = 3172       -- the LOT for the inventor building
CONSTANTS["SWITCH_THROWER_LOT"] = 3923          -- the LOT for the switch thrower
CONSTANTS["HAZMAT_VAN_LOT"] = 3472              -- the LOT for the animating hazmat van
CONSTANTS["HAZMAT_REBUILD_VAN_LOT"] = 3717      -- the LOT for the hazmat rebuild van
CONSTANTS["BUBBLE_BLOWER_LOT"] = 3928           -- the LOT for the bubble blower
CONSTANTS["AIR_STINK_LOT"] = 3645				-- the LOT for the mid-air stink effects
CONSTANTS["SPAWNED_HAZMAT_NPC"] = 3553          -- the LOT for the spawned hazmat npcs
CONSTANTS["POLE_SLIDE_NPC"] = 3954          	-- the LOT for the spawned window washer
CONSTANTS["BALLOON_LOT"] = 3433          		-- the LOT for the balloon
CONSTANTS["FLOWER_LOT"] = 3646           		-- the LOT for the flowers

CONSTANTS["INVASION_SKUNK_LOT"] = {3279, 3930, 3931} -- the LOTs for invasion skunks

CONSTANTS["INVASION_PANIC_ACTORS"] = {3268, 3269, 3270, 3271, 3272}


--------------------------------------------------------------
-- Invasion Constants
--------------------------------------------------------------
-- States
CONSTANTS["ZONE_STATE_NO_INFO"]         = -1   -- we have not been given state information yet
CONSTANTS["ZONE_STATE_NO_INVASION"]     = 0    -- no invasion
CONSTANTS["ZONE_STATE_TRANSITION"]      = 1    -- transition into the invasion
CONSTANTS["ZONE_STATE_HIGH_ALERT"]      = 2    -- invasion at high alert
CONSTANTS["ZONE_STATE_MEDIUM_ALERT"]    = 3    -- invasion at medium alert
CONSTANTS["ZONE_STATE_LOW_ALERT"]       = 4    -- invasion at low alert
CONSTANTS["ZONE_STATE_DONE_TRANSITION"] = 5    -- transition to no invasion

-- Scoring
CONSTANTS["CLEANING_POINTS_TOTAL"]   = 50  	-- number of cleaning points to stop invasion
CONSTANTS["CLEANING_POINTS_MEDIUM"]  = 20   -- number of cleaning points to move to medium alert
CONSTANTS["CLEANING_POINTS_LOW"]     = 40   -- number of cleaning points to move to low alert
CONSTANTS["POINT_VALUE_SKUNK"]       = 1    -- the point value for cleaning a skunk
CONSTANTS["POINT_VALUE_STINK_CLOUD"] = 1    -- the point value for cleaning a stink cloud
CONSTANTS["POINT_VALUE_BROOMBOT"]    = 3    -- the point value for repairing a broombot
CONSTANTS["POINT_VALUE_HAZMAT"]      = 2    -- the point value for cleaning a hazmat
CONSTANTS["REWARD_MULTIPLIER"]       = 10   -- multiplied by the number of points a player has for reward in coins

-- Skunks / Stink Clouds
CONSTANTS["NUM_SKUNKS"]             = 10   -- number of skunks to spawn during invasion
CONSTANTS["SKUNK_PATH_PREFIX"]      = "skunkWP_"  -- prefix for skunk paths
CONSTANTS["SKUNK_ROAM_PATH_SUFFIX"] = "a"         -- suffix appended to the previous path for a roaming path
CONSTANTS["NUM_STINK_CLOUDS"]       = 10   -- number of stink clouds to spawn during invasion
CONSTANTS["STINK_CLOUD_PATH"]       = "StinkCloudSpawnLocations"
CONSTANTS["SKUNK_RESPAWN_TIMER_MIN"] = 5   -- min seconds to respawn
CONSTANTS["SKUNK_RESPAWN_TIMER_MAX"] = 10  -- max seconds to respawn

-- Hazmat Van / Hazmat NPCs
CONSTANTS["HAZMAT_REBUILD_VAN_SPAWN_PATH"] = "HazmatRebuildSpawnPath" -- path to spawn the hazmat van on (first waypoint)
CONSTANTS["HAZMAT_NPC_PATH_PREFIX"]        = "hazmatWP_"              -- prefix for hazmat npc paths
CONSTANTS["NUM_HAZMAT_NPCS"]               = 4                        -- number of hazmat npcs to spawn during invasion
CONSTANTS["TIME_BETWEEN_HAZMAT_SPAWNS"]    = 4.0    -- time in between hazmat NPC spawns
CONSTANTS["HAZMAT_REBUILD_RESET_TIME"]     = 20.0   -- time until rebuild van breaks


--------------------------------------------------------------
-- Invasion Timings
--------------------------------------------------------------
CONSTANTS["PEACE_TIME_DURATION"] = 5 * 60       -- duration of peace in town, event starts afterwards
CONSTANTS["INVASION_TRANSITION_DURATION"]= 12.0 -- Duration of the transition to the invasion
CONSTANTS["DONE_TRANSITION_DURATION"] = 5.0     -- Duration of the transition from the invasion
CONSTANTS["MAX_INVASION_DURATION"] = 5 * 60.0   -- maximum duration of the invasion regardless of cleaning points
CONSTANTS["EARTHQUAKE_DURATION"] = 2.5          -- Time from start of event to do things after earthquake
CONSTANTS["FOUNTAIN_ALERT_TIMING"] = 4.0        -- Time from start of event to trigger fountain alert
CONSTANTS["SKUNK_SPAWN_TIMING"] = 10.0          -- Time from start of event to trigger skunk spawns and sky
CONSTANTS["HAZMAT_VAN_TIMING"] = 11.0           -- Time from start of event to trigger van animations
CONSTANTS["POLE_SLIDE_TIMING"] = 11.0           -- Time from start of event to trigger pole slide animations
CONSTANTS["HAZMAT_NPC_SPAWN_TIMER"] = 1.0       -- Time from buildable van spawning to start Hazmat NPCs


--------------------------------------------------------------
-- Dynamic Skybox Information
--------------------------------------------------------------
CONSTANTS["STINKY_SKYBOX"] = "mesh/env/env_sky_won_yore_skunk-stink.nif"
CONSTANTS["NORMAL_SKYBOX"] = "mesh/env/challenge_sky_light_2awesome.nif"
CONSTANTS["SKYLAYER"] = "(invalid)"
CONSTANTS["RINGLAYER0"] = "(invalid)"
CONSTANTS["RINGLAYER1"] = "(invalid)"
CONSTANTS["RINGLAYER2"] = "(invalid)"
CONSTANTS["RINGLAYER3"] = "(invalid)"


--------------------------------------------------------------
-- General Bouncer Information
--------------------------------------------------------------
CONSTANTS["HF_NODE_BOUNCER"] = 7
CONSTANTS["HF_SUB_ITEM_SEP_STRING"] = "\x1F"


--------------------------------------------------------------
-- Spout Information
--------------------------------------------------------------
CONSTANTS["SPOUT_RADIUS"]= 2.0	-- do not increase this or the spout won't realize it has become temporarily unplugged if a player jumps while standing on it
CONSTANTS["SPOUT_GROUP_NAME"] = "spoutGroup"
CONSTANTS["FOUNTAIN_GROUP_NAME"] = "fountainGroup"
CONSTANTS["SPOUT_BOUNCER_SPEED"] = 100.0
CONSTANTS["SPOUT_BOUNCER_DEST"] = {x = -12.88, y = 318.21, z = -124.52}


--------------------------------------------------------------
-- Bubble Statue Information
--------------------------------------------------------------
CONSTANTS["BUBBLE_STATUE_RADIUS"] = 10.0


--------------------------------------------------------------
-- Inventor's Balloon Information
--------------------------------------------------------------
CONSTANTS["LAST_BALLOON_WAYPOINT"] = 11


--------------------------------------------------------------
-- general
--------------------------------------------------------------
CONSTANTS["radius"] = 3.0
