--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('o_ShootingGallery')
require('ai/MINIGAME/SG_GF/L_GF_SG')

-- @TODO: Add Path Changing on waypoint, need to get [Closest Waypoint on New Path]
--        Also need [Number of Waypoints on Path]
-- @TODO: Optimize

--------------------------------------------------------------
-- Locals and Constants
LOCALS = {}
CONSTANTS = {}
STORECONSTANTS = {}
--------------------------------------------------------------

-- Start Location for the Zone
CONSTANTS["PLAYER_START_POSx"] =  -446.288849
CONSTANTS["PLAYER_START_POSy"] =  277.685211
CONSTANTS["PLAYER_START_POSz"] =  56.183247


CONSTANTS["PLAYER_START_ROTw"] = 0.91913521289825
CONSTANTS["PLAYER_START_ROTx"] = 0
CONSTANTS["PLAYER_START_ROTy"] = 0.39394217729568
CONSTANTS["PLAYER_START_ROTz"] = 0


-- cannon constants
CONSTANTS["CANNON_TEMPLATEID"] = 1864
CONSTANTS["IMPACT_SKILLID"] = 34

 CONSTANTS["PROJECTILE_TEMPLATEID"] = 1822
CONSTANTS["CANNON_PLAYER_OFFSETx"]  = 6.652
CONSTANTS["CANNON_PLAYER_OFFSETy"]  = 0
CONSTANTS["CANNON_PLAYER_OFFSETz"]  = -1.188


CONSTANTS["CANNON_VELOCITY"] = 129.0

CONSTANTS["CANNON_MIN_DISTANCE"] = 30.0
CONSTANTS["CANNON_REFIRE_RATE"] = 800.0

CONSTANTS["CANNON_BARREL_OFFSETx"] = 0
CONSTANTS["CANNON_BARREL_OFFSETy"] = 4.3
CONSTANTS["CANNON_BARREL_OFFSETz"] = 9


CONSTANTS["CANNON_TIMEOUT"] = -1
CONSTANTS["CANNON_FOV"] = 50.0
CONSTANTS["CANNON_USE_LEADERBOARDS"] = true
CONSTANTS["STREAK_MOD"] = 2

-- for Animations
STORECONSTANTS["VALID_ACTORS"] = {3109, 3110, 3111, 3112, 3125, 3126}
STORECONSTANTS["STREAK_BONUS"] = {1,2,5,10}
STORECONSTANTS["VALID_EFFECTS"] = {3122}

-- Super Charger  is charged for this amount of time
CONSTANTS["ChargedTime"] = 10
-- The amount of points needed to supper charge
CONSTANTS["ChargedPoints"] = 25000

-- Activity ID 
CONSTANTS["ActivityID"] = 4

CONSTANTS["Reward_Model_GrpName"] = "QBRewardGroup"


ACTORS = {}					-- stores actors for the instance
EFFECTS = {}					-- stores effects for the instance
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
STORECONSTANTS["NUM_WAVES"] = #waves
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
	self:SetVar("StreakBouns",0)
	self:SetVar("StopCharge", false )
	self:SetVar("NumberOfCharges", 0 ) 
	wave1Score = 0
	wave2Score = 0
	wave3Score = 0	
    self:SetVar("WaveStatus", true)
    self:SetVar("StreakBouns", 0)
    setCONSTANTS( self,spawns,SPAWN_DATA,CONSTANTS , waves,  PLAYER_SCORE, EFFECTS , ACTORS,STORECONSTANTS  )
    self:SetVar("CONSTANTS", CONSTANTS)
    self:SetVar("timelimit", waves[1].timeLimit)
    totalscore = 0
end


--------------------------------------------------------------
-- target Settings
--------------------------------------------------------------
function targetSettings(self)
    -- AddPath - adds a path to a spawn. Can have one or more.
    -- Syntax:  AddPath(paths, "[Path Name]")

    --
    -- NOTE: All times are in Seconds (i.e. 3.50 = 3 and a half seconds)
    --------------------------------------------------------------
    -- Wave 1 Spawns
    spawn = {}
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Obj_1")
    AddPath(paths,	"Wave_1_Obj1_2")
	AddSpawn(spawn,paths,
		 6015,	--[Template ID]
		 0.0,	--[Initial Spawn Time Min]
		 2.0,	--[Initial Spawn Time Max]
		 true,	--[Does Respawn]
		 0,		--[Respawn Time Min],
		 2,	    --[Respawn Time Max]
		 2.0,	--[Initial Speed]
		 10000,	--[Score]	
		 false,	--[Change Speed at Waypoints]
		 0.0,	--[Chance to Change Speed]
		 1.0,	--[Min Speed]
		 1.0,	--[Max Speed]
		 false,	--[Is Moving Platform]	
		 true,	--[Despawn On Last Waypoint]	
		 0.0)	--[Time Score]
    
    AddSpawnsForWave(spawns,spawn)
    -- Wave 2 Spawns
    spawn = {}
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Obj_1")
    AddPath(paths,	"Wave_1_Obj1_2")
	AddSpawn(spawn,paths,
		 5946,	--[Template ID]
		 0.0,	--[Initial Spawn Time Min]
		 2.0,	--[Initial Spawn Time Max]
		 true,	--[Does Respawn]
		 0,		--[Respawn Time Min],
		 2,	    --[Respawn Time Max]
		 2.0,	--[Initial Speed]
		 10000,	--[Score]	
		 false,	--[Change Speed at Waypoints]
		 0.0,	--[Chance to Change Speed]
		 1.0,	--[Min Speed]
		 1.0,	--[Max Speed]
		 false,	--[Is Moving Platform]	
		 true,	--[Despawn On Last Waypoint]	
		 0.0)	--[Time Score]
    
    AddSpawnsForWave(spawns,spawn)
    -- Wave 3 Spawns
    spawn = {}
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Obj_1")
    AddPath(paths,	"Wave_1_Obj1_2")
	AddSpawn(spawn,paths,
		 6300,	--[Template ID]
		 0.0,	--[Initial Spawn Time Min]
		 2.0,	--[Initial Spawn Time Max]
		 true,	--[Does Respawn]
		 0,		--[Respawn Time Min],
		 2,	    --[Respawn Time Max]
		 2.0,	--[Initial Speed]
		 10000,	--[Score]	
		 false,	--[Change Speed at Waypoints]
		 0.0,	--[Chance to Change Speed]
		 1.0,	--[Min Speed]
		 1.0,	--[Max Speed]
		 false,	--[Is Moving Platform]	
		 true,	--[Despawn On Last Waypoint]	
		 0.0)	--[Time Score]
    
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

function onPlayerExit( self, msg)
    if (msg) then
        mainPlayerExit( self, msg)
    end
end
function onShootingGalleryFire(self, msg)
    if (msg) then
        mainShootingGalleryFire( self, msg)
    end
end
