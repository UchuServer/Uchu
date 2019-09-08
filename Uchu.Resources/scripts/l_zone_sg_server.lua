--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('o_ShootingGallery')
require('L_GF_SG')

-- @TODO: Add Path Changing on waypoint, need to get [Closest Waypoint on New Path]
--        Also need [Number of Waypoints on Path]
-- @TODO: Optimize

--------------------------------------------------------------
-- Locals and Constants
CONSTANTS = {}
TABLES = {}
LOCALS = {}
--------------------------------------------------------------

-- Start Location for the Zone
CONSTANTS["PLAYER_START_POSx"] =  -908.542480
CONSTANTS["PLAYER_START_POSy"] =   229.773178
CONSTANTS["PLAYER_START_POSz"] =  -908.542480


CONSTANTS["PLAYER_START_ROTw"] = 0.91913521289825
CONSTANTS["PLAYER_START_ROTx"] = 0
CONSTANTS["PLAYER_START_ROTy"] = 0.39394217729568
CONSTANTS["PLAYER_START_ROTz"] = 0


-- cannon constants
CONSTANTS["CANNON_TEMPLATEID"] = 1864
CONSTANTS["IMPACT_SKILLID"] = 34

CONSTANTS["PROJECTILE_TEMPLATEID"] = 1822
CONSTANTS["CANNON_PLAYER_OFFSETx"] = 6.652
CONSTANTS["CANNON_PLAYER_OFFSETy"] = -2
CONSTANTS["CANNON_PLAYER_OFFSETz"] = 1.5 
CONSTANTS["Reward_Model_Matrix"] = 157


CONSTANTS["CANNON_VELOCITY"] = 129.0

CONSTANTS["CANNON_MIN_DISTANCE"] = 30.0
CONSTANTS["CANNON_REFIRE_RATE"] = 800.0

CONSTANTS["CANNON_BARREL_OFFSETx"] = 0
CONSTANTS["CANNON_BARREL_OFFSETy"] = 4.3
CONSTANTS["CANNON_BARREL_OFFSETz"] = 9

CONSTANTS["CANNON_SUPER_CHARGE"] = 6297 
CONSTANTS["CANNON_PROJECTILE"] = 1822 
CONSTANTS["CANNON_SUPERCHARGE_SKILL"] = 249
CONSTANTS["CANNON_SKILL"] = 228


CONSTANTS["CANNON_TIMEOUT"] = -1
CONSTANTS["CANNON_FOV"] = 58.6
CONSTANTS["CANNON_USE_LEADERBOARDS"] = true
CONSTANTS["STREAK_MOD"] = 2

-- for Animations
TABLES["VALID_ACTORS"] = {3109, 3110, 3111, 3112, 3125, 3126}
TABLES["STREAK_BONUS"] = {1,2,5,10}
TABLES["VALID_EFFECTS"] = {3122}

-- Super Charger  is charged for this amount of time
CONSTANTS["ChargedTime"] = 10
-- The amount of points needed to supper charge
CONSTANTS["ChargedPoints"] = 25000
-- Modle reward grp name
CONSTANTS["Reward_Model_GrpName"] = "QBRewardGroup"
-- Activity ID 
CONSTANTS["ActivityID"] = 1864
-- Activity ID 

-- Reward Score and Loot Matrix\
-- 1
CONSTANTS["Score_Reward_1"] = 50000
CONSTANTS["Score_LootMatrix_1"] = 157
-- 2
CONSTANTS["Score_Reward_2"] = 100000
CONSTANTS["Score_LootMatrix_2"] = 187
-- 3
CONSTANTS["Score_Reward_3"] = 200000
CONSTANTS["Score_LootMatrix_3"] = 188
-- 4
CONSTANTS["Score_Reward_4"] = 400000
CONSTANTS["Score_LootMatrix_4"] = 189
-- 5
CONSTANTS["Score_Reward_5"] = 800000
CONSTANTS["Score_LootMatrix_5"] = 190


--------------------------------------------------------------
-- Wave Data
waves = {}
PLAYER_SCORE = {}
--------------------------------------------------------------
-- Syntax:     [Time]   [Text to Show Player]
--------------------------------------------------------------
AddWave(waves,	30.0,	"Wave;One"   )		
AddWave(waves,	30.0,	"Wave;Two" )
AddWave(waves,	30.0,	"Wave;Three" )
-- wave constants
CONSTANTS["NUM_WAVES"] = #waves
CONSTANTS["FIRST_WAVE_START_TIME"] = 4.0
CONSTANTS["IN_BETWEEN_WAVE_PAUSE"] = 7.0

--------------------------------------------------------------
-- Spawn Data
spawns = {}
SPAWN_DATA = {}
--------------------------------------------------------------
-- Startup do not chaged!!!
--------------------------------------------------------------
function onStartup(self)
    targetSettings(self)
	-- set game state
	LOCALS["GameStarted"] = false
	LOCALS["CurSpawnNum"] = 0
	LOCALS["ThisWave"] = 0
	LOCALS["GameScore"] = 0
	LOCALS["GameTime"] = 0
	LOCALS["NumShots"] = 0
	LOCALS["NumKills"] = 0
	LOCALS["MaxStreak"] = 0
	--self:SetVar("StreakBonus",0)
	self:SetVar("StopCharge", false )
	self:SetVar("NumberOfCharges", 0 ) 
	wave1Score = 0
	wave2Score = 0
	wave3Score = 0	
    self:SetVar("WaveStatus", true)
    --self:SetVar("StreakBonus", 0)
    self:SetVar("CONSTANTS", CONSTANTS)
    self:SetVar("timelimit", waves[1].timeLimit)
    totalscore = 0
    
    --setCONSTANTS(self, spawns, SPAWN_DATA, CONSTANTS, waves, PLAYER_SCORE, EFFECTS, ACTORS, LOCALS )  
end


--------------------------------------------------------------
-- target Settings
--------------------------------------------------------------
function targetSettings(self)
    -- AddPath - adds a path to a spawn. Can have one or more.
    -- Syntax:  AddPath(paths, "[Path Name]")


    -- AddSpawn - configures data for the spawn and adds it. Can
    --            only have one.
    -- Syntax: AddSpawn(spawn, paths, [Template ID],
    --                  [Initial Spawn Time Min], [Initial Spawn Time Max], [Does Respawn],
    --                  [Respawn Time Min], [Respawn Time Max],
    --                  [Initial Speed], [Score], [Change Speed at Waypoints],
    --                  [Chance to Change Speed], [Min Speed], [Max Speed],
    --                  [Is Moving Platform], [Despawn On Last Waypoint], [Time Score])
    --
    -- NOTE: All times are in Seconds (i.e. 3.50 = 3 and a half seconds)
    --------------------------------------------------------------
    -- Wave 1 Spawns
spawn = {}
    --------------------------------------------------------------
    
  
    -- Ship 1
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Ship_1")
    AddPath(paths,	"Wave_1_Ship_3")
	AddSpawn(spawn,paths,
		 6015,	0.0,	2.0,	true,	0,	2,	2.0,	1500,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
         

    -- Ship 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	
    AddPath(paths,	"Wave_1_Ship_2")
    AddPath(paths,	"Wave_1_Ship_4")
	AddSpawn(spawn,paths,
		 6300,	0.0,	2.0,	true,	0,	2,	2.0,	500,		false,	0.0,	1.0,	1.0,	false,	    true,		0.0)

	-- Sub
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Sub_1")
	AddPath(paths,	"Wave_1_Sub_2")
	AddSpawn(spawn,paths,
		 6016,	0.0,	2.0,	true,	0,	2,	10.0,	1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
    
    -- Sub 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_2_Sub_1")
	AddPath(paths,	"Wave_2_Sub_2")
	AddSpawn(spawn,paths,
		 6016,	0.0,	2.0,	true,	0,	2,	2.0,	1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)

    -- Friendly
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_FShip_1")
	AddPath(paths,	"Wave_3_FShip_2")
	AddSpawn(spawn,paths,
		 2168,	0.0,	5.0,	true,	2.0,	5.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
    

    -- Add the spawns for the wave
    AddSpawnsForWave(spawns,spawn)


    --------------------------------------------------------------
    -- Wave 2 Spawns
    spawn = {}
    --------------------------------------------------------------
    

    -- Ship 1
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Ship_1")
    AddPath(paths,	"Wave_1_Ship_3")
	AddSpawn(spawn,paths,
		 6015,	0.0,	2.0,	true,	0,	2,	2.0,	1500,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
    
    -- Ship 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Ship_2")
    AddPath(paths,	"Wave_1_Ship_4")
	AddSpawn(spawn,paths,
		 6300,	0.0,	2.0,	true,	0,	2,	2.0,	500,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
    
    -- Ship 3
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_2_Ship_1")
	AddSpawn(spawn,paths,
		 6300,	0.0,	2.0,	true,	0,	2,	2.0,	500,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
    -- Ship 4
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
    AddPath(paths,	"Wave_2_Ship_2")
	AddSpawn(spawn,paths,
		 6015,	0.0,	2.0,	true,	0,	2,	2.0,	1500,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)

	-- Sub 1
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Sub_1")
	AddPath(paths,	"Wave_1_Sub_2")
	AddSpawn(spawn,paths,
		 6016,	0.0,	2.0,	true,	0,	2,	2.0,	1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
    
    -- Sub 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_2_Sub_1")
	AddPath(paths,	"Wave_2_Sub_2")
	AddSpawn(spawn,paths,
		 6016,	0.0,	2.0,	true,	0,	2,	2.0,	1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)

    -- Duck
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Duck_1")
	AddPath(paths,	"Wave_1_Duck_2")
    AddSpawn(spawn,paths,
		 5946,	5.0,	10.0,	true,	5.0,	10.0,	4.0,	5000,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
    
    -- Friendly
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_FShip_1")
	AddPath(paths,	"Wave_3_FShip_2")
	AddSpawn(spawn,paths,
		 2168,	0.0,	5.0,	true,	2.0,	5.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)


    -- Add the spawns for the wave
    AddSpawnsForWave(spawns,spawn)


    --------------------------------------------------------------
    -- Wave 3 Spawns
    spawn = {}
    --------------------------------------------------------------	

    
	-- Ship 1
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Ship_1")
    AddPath(paths,	"Wave_1_Ship_3")
	AddSpawn(spawn,paths,
		 6015,	0.0,	2.0,	true,	0,	2,	2.0,	1500,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
    
    -- Ship 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
    AddPath(paths,	"Wave_1_Ship_2")
    AddPath(paths,	"Wave_1_Ship_4")
	AddSpawn(spawn,paths,
		 6300,	0.0,	2.0,	true,	0,	2,	2.0,	500,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
    
    -- Ship 3
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_2_Ship_1")
    AddPath(paths,	"Wave_2_Ship_2")
	AddSpawn(spawn,paths,
		 6015,	0.0,	2.0,	true,	0,	2,	2.0,	500,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
    
    -- Ship 4
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Ship_1")
    AddPath(paths,	"Wave_3_Ship_2")
	AddSpawn(spawn,paths,
		 6300,	0.0,	2.0,	true,	0,	2,	2.0,	1500,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)

	-- Sub 1
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Sub_1")
	AddPath(paths,	"Wave_1_Sub_2")
	AddSpawn(spawn,paths,
		 6016,	0.0,	2.0,	true,	0,	2,	2.0,	1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
    
    -- Sub 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_2_Sub_1")
	AddPath(paths,	"Wave_2_Sub_2")
	AddSpawn(spawn,paths,
		 6016,	0.0,	2.0,	true,	0,	2,	2.0,	1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
    
    -- Sub 3
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Sub_1")
	AddPath(paths,	"Wave_3_Sub_2")
	AddSpawn(spawn,paths,
		 6016,	0.0,	2.0,	true,	0,	2,	2.0,	1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)

    -- Duck
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Duck_1")
	AddPath(paths,	"Wave_1_Duck_2")
    AddSpawn(spawn,paths,
		 5946,	5.0,	10.0,	true,	5.0,	10.0,	4.0,	5000,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
    
    -- Ness
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Ness_1")
	AddPath(paths,	"Wave_1_Ness_2")
    AddPath(paths,	"Wave_2_Ness_1")
	AddSpawn(spawn,paths,
		 2565,	10.0,	15.0,	true,	10.0,	15.0,	2.0,	10000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
    
    -- Friendly
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_FShip_1")
	AddPath(paths,	"Wave_3_FShip_2")
	AddSpawn(spawn,paths,
		 2168,	0.0,	5.0,	true,	2.0,	5.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)
    
    -- Friendly 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_FShip_3")
	AddPath(paths,	"Wave_3_FShip_4")
	AddSpawn(spawn,paths,
		 2168,	0.0,	5.0,	true,	2.0,	5.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	false,		true,		0.0)

   

    -- Add the spawns for the wave
    AddSpawnsForWave(spawns,spawn)


end

----------------------------------------------------------------------------------------------------------------------------
-- Do not Chage -- 
----------------------------------------------------------------------------------------------------------------------------

function onPlayerLoaded(self, msg)
    if (msg) then
        mainPlayerLoaded(self, msg)
    end
end

function onObjectLoaded(self, msg)
    if (msg) then
     mainObjectLoaded(self, msg)
    end
end

function onNotifyObject(self, msg)
    if( msg) then
        mainNotifyObject(self, msg)
    end
end
