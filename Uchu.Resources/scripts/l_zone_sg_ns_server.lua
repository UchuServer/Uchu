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
LOCALS = {}
CONSTANTS = {}
STORECONSTANTS = {}
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
CONSTANTS["CANNON_TEMPLATEID"] = 7583
CONSTANTS["IMPACT_SKILLID"] = 396

CONSTANTS["PROJECTILE_TEMPLATEID"] = 7833
CONSTANTS["CANNON_PLAYER_OFFSETx"] = 6.652
CONSTANTS["CANNON_PLAYER_OFFSETy"] = 0
CONSTANTS["CANNON_PLAYER_OFFSETz"] = -1.188
CONSTANTS["Reward_Model_Matrix"] = 157

CONSTANTS["CANNON_VELOCITY"] = 220.0

CONSTANTS["CANNON_MIN_DISTANCE"] = 130.0
CONSTANTS["CANNON_REFIRE_RATE"] = 400 --800

CONSTANTS["CANNON_BARREL_OFFSETx"] = 0
CONSTANTS["CANNON_BARREL_OFFSETy"] = 4.3
CONSTANTS["CANNON_BARREL_OFFSETz"] = 9

CONSTANTS["CANNON_SUPER_CHARGE"] = 7849 
CONSTANTS["CANNON_PROJECTILE"] = 1822
CONSTANTS["CANNON_SUPERCHARGE_SKILL"] = 398
CONSTANTS["CANNON_SKILL"] = 397 

CONSTANTS["CANNON_TIMEOUT"] = -1
CONSTANTS["CANNON_FOV"] = 25 --26 
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
-- Modle reward grp name
CONSTANTS["Reward_Model_GrpName"] = "QBRewardGroup"
-- Activity ID 
CONSTANTS["ActivityID"] = 7583
-- Activity ID 

-- Reward Score and Loot Matrix\
-- 1
CONSTANTS["Score_Reward_1"] = 50000
CONSTANTS["Score_LootMatrix_1"] = 279
-- 2
CONSTANTS["Score_Reward_2"] = 100000
CONSTANTS["Score_LootMatrix_2"] = 280
-- 3
CONSTANTS["Score_Reward_3"] = 200000
CONSTANTS["Score_LootMatrix_3"] = 281
-- 4
CONSTANTS["Score_Reward_4"] = 400000
CONSTANTS["Score_LootMatrix_4"] = 282
-- 5
CONSTANTS["Score_Reward_5"] = 800000
CONSTANTS["Score_LootMatrix_5"] = 283

local targetEscapedValue = -1000

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
CONSTANTS["IN_BETWEEN_WAVE_PAUSE"] = 4.0

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
    
  
    -- Ducks 1
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_1")
	AddPath(paths,	"Row_2_Ducks_1")
    AddPath(paths,	"Row_3_Ducks_1")
	AddSpawn(spawn,paths,
		 6920,	0.0,	1.0,	true,	2.0,	8.0,	2.0,	1500,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
        
	-- Ducks 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_2_Ducks_3")
    AddPath(paths,	"Row_3_Ducks_3")
	AddSpawn(spawn,paths,
		 6919,	0.0,	1.0,	true,	2.0,	8.0,	10.0,	1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
    		 
    -- Ducks 3
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_1")
	AddPath(paths,	"Row_2_Ducks_2")
    AddPath(paths,	"Row_3_Ducks_3")
	AddSpawn(spawn,paths,
		 6920,	0.0,	2.0,	true,	2.0,	8.0,	2.0,	1500,		true,	0.5,	1.0,	3.0,	true,		true,		0.0)
         
	-- Ducks 4
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
    AddPath(paths,	"Row_1_Ducks_3")
	AddPath(paths,	"Row_2_Ducks_3")
    AddPath(paths,	"Row_3_Ducks_3")
	AddSpawn(spawn,paths,
		 6918,	1.0,	3.0,	true,	2.0,	8.0,	10.0,	1000,		true,	0.5,	1.0,	3.0,	true,		true,		0.0)  

    -- Bulls Eye
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_2_5")
	AddPath(paths,	"Mid_Row_3_5")
	AddSpawn(spawn,paths,
		 7801,	15.0,	20.0,	false,	10.0,	15.0,	1.0,	5000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)

	--************************************************************	
	--------------------------------------------------------------
    -- Friendly 1
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_1_1")
	AddPath(paths,	"Mid_Row_1_2")
	AddPath(paths,	"Mid_Row_1_3")
	AddPath(paths,	"Mid_Row_1_4")
	AddSpawn(spawn,paths,
		 7800,	1.0,	8.0,	false,	2.0,	8.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
	-- Friendly 2
	----------------------------------------------------------
	paths = {}
	----------------------------------------------------------
	AddPath(paths,	"Mid_Row_2_1")
	AddPath(paths,	"Mid_Row_2_2")
	AddPath(paths,	"Mid_Row_2_3")
	AddPath(paths,	"Mid_Row_2_4")
	AddSpawn(spawn,paths,
		 7800,	5.0,	12.0,	false,	2.0,	8.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)    

    -- Friendly 3
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
    AddPath(paths,	"Mid_Row_3_1")
	AddPath(paths,	"Mid_Row_3_2")
	AddSpawn(spawn,paths,
		 7800,	10.0,	18.0,	false,	2.0,	8.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
	-- Friendly 4
	----------------------------------------------------------
	paths = {}
	----------------------------------------------------------
    AddPath(paths,	"Mid_Row_3_3")
	AddPath(paths,	"Mid_Row_3_4")
	AddSpawn(spawn,paths,
		 7800,	15.0,	20.0,	false,	2.0,	8.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
    

    -- Add the spawns for the wave
    AddSpawnsForWave(spawns,spawn)


    --------------------------------------------------------------
    -- Wave 2 Spawns
    spawn = {}
    --------------------------------------------------------------
    

    -- Ducks 1
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_1")
	AddPath(paths,	"Row_2_Ducks_1")
    AddPath(paths,	"Row_3_Ducks_1")
	AddSpawn(spawn,paths,
		 6918,	0.0,	1.0,	true,	2.0,	8.0,	2.0,	1500,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
         
    -- Ducks 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_2")
	AddPath(paths,	"Row_2_Ducks_2")
    AddPath(paths,	"Row_3_Ducks_2")
	AddSpawn(spawn,paths,
		 6920,	0.0,	1.0,	true,	2.0,	8.0,	2.0,	500,		false,	0.0,	1.0,	1.0,	true,	    true,		0.0)

	-- Ducks 3
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_3")
	AddPath(paths,	"Row_2_Ducks_3")
    AddPath(paths,	"Row_3_Ducks_3")
    AddPath(paths,	"Row_Switch_2_1")
    AddPath(paths,	"Row_Switch_2_1")
	AddSpawn(spawn,paths,
		 6919,	0.0,	2.0,	true,	2.0,	8.0,	10.0,	1000,		true,	0.25,	1.0,	3.0,	true,		true,		0.0)

    -- Ducks 4
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_1")
	AddPath(paths,	"Row_2_Ducks_1")
    AddPath(paths,	"Row_3_Ducks_1")
	AddSpawn(spawn,paths,
		 6918,	0.0,	2.0,	true,	2.0,	8.0,	2.0,	1500,		true,	0.25,	1.0,	4.0,	true,		true,		0.0)
         
    -- Ducks 5
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_2")
	AddPath(paths,	"Row_2_Ducks_2")
    AddPath(paths,	"Row_3_Ducks_2")
	AddSpawn(spawn,paths,
		 6920,	1.0,	3.0,	true,	2.0,	8.0,	2.0,	500,		true,	0.5,	1.0,	3.0,	true,	    true,		0.0)

	-- Ducks 6
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_3")
	AddPath(paths,	"Row_2_Ducks_3")
    AddPath(paths,	"Row_3_Ducks_3")
    AddPath(paths,	"Row_Switch_2_2")
    AddPath(paths,	"Row_Switch_2_2")
	AddSpawn(spawn,paths,
		 6919,	1.0,	4.0,	true,	2.0,	8.0,	10.0,	1000,		true,	0.75,	0.5,	4.0,	true,		true,		0.0)

    
    -- Bulls Eye
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_1_3")
	AddPath(paths,	"Mid_Row_2_5")
	AddPath(paths,	"Mid_Row_3_5")
	AddSpawn(spawn,paths,
		 7801,	1.0,	8.0,	false,	10.0,	15.0,	1.0,	5000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
    -- Bulls Eye 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_2_7")
	AddPath(paths,	"Mid_Row_3_7")
	AddPath(paths,	"Mid_Row_1_7")
	AddPath(paths,	"Mid_Row_2_6")
	AddPath(paths,	"Mid_Row_3_6")
	AddPath(paths,	"Mid_Row_1_6")
	AddSpawn(spawn,paths,
		 7801,	10.0,	15.0,	false,	10.0,	15.0,	5.0,	5000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
	
	
	--************************************************************
	--------------------------------------------------------------
	-- Friendly
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_1_1")
	AddPath(paths,	"Mid_Row_1_2")
	AddPath(paths,	"Mid_Row_1_3")
	AddPath(paths,	"Mid_Row_1_4")
	AddSpawn(spawn,paths,
		 7800,	10.0,	18.0,	false,	10.0,	15.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_2_1")
	AddPath(paths,	"Mid_Row_2_2")
	AddPath(paths,	"Mid_Row_2_3")
	AddPath(paths,	"Mid_Row_2_4")
	AddSpawn(spawn,paths,
		 7800,	21.0,	27.0,	false,	10.0,	15.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
    -- Friendly 3
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_3_1")
	AddPath(paths,	"Mid_Row_3_2")
	AddSpawn(spawn,paths,
		 7800,	1.0,	8.0,	false,	2.0,	4.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
	-- Friendly 4
	----------------------------------------------------------
	paths = {}
	----------------------------------------------------------
	AddPath(paths,	"Mid_Row_3_3")
	AddPath(paths,	"Mid_Row_3_4")
	AddSpawn(spawn,paths,
		 7800,	5.0,	10.0,	false,	2.0,	5.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)

    -- Add the spawns for the wave
    AddSpawnsForWave(spawns,spawn)


    --------------------------------------------------------------
    -- Wave 3 Spawns
    spawn = {}
    --------------------------------------------------------------	

    
    -- Ducks 1
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_1")
	AddPath(paths,	"Row_2_Ducks_1")
    AddPath(paths,	"Row_3_Ducks_1")
	AddSpawn(spawn,paths,
		 6919,	0.0,	2.0,	true,	2.0,	8.0,	2.0,	1500,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
         

    -- Ducks 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_2")
	AddPath(paths,	"Row_2_Ducks_2")
    AddPath(paths,	"Row_3_Ducks_2")
	AddSpawn(spawn,paths,
		 6918,	0.0,	2.0,	true,	2.0,	8.0,	2.0,	500,		false,	0.0,	1.0,	1.0,	true,	    true,		0.0)

	-- Ducks 3
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_3")
	AddPath(paths,	"Row_2_Ducks_3")
    AddPath(paths,	"Row_3_Ducks_3")
	AddSpawn(spawn,paths,
		 6920,	0.0,	2.0,	true,	2.0,	8.0,	10.0,	1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
		 
	-- Ducks 4
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_4")
	AddPath(paths,	"Row_2_Ducks_4")
    AddPath(paths,	"Row_3_Ducks_4")
	AddSpawn(spawn,paths,
		 6919,	0.0,	2.0,	true,	2.0,	8.0,	10.0,	1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
	-- Ducks 5
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_3")
	AddPath(paths,	"Row_2_Ducks_3")
    AddPath(paths,	"Row_3_Ducks_3")
    AddPath(paths,	"Row_Switch_2_1")
    AddPath(paths,	"Row_Switch_2_1")
	AddSpawn(spawn,paths,
		 6919,	0.0,	2.0,	true,	2.0,	8.0,	10.0,	1000,		true,	0.25,	1.0,	3.0,	true,		true,		0.0)

    -- Ducks 6
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_1")
	AddPath(paths,	"Row_2_Ducks_1")
    AddPath(paths,	"Row_3_Ducks_1")
    AddPath(paths,	"Row_Switch_2_2")
    AddPath(paths,	"Row_Switch_2_2")
	AddSpawn(spawn,paths,
		 6918,	0.0,	2.0,	true,	2.0,	8.0,	5.0,	1500,		true,	0.25,	1.0,	4.0,	true,		true,		0.0)
         
    -- Ducks 7
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_2")
	AddPath(paths,	"Row_2_Ducks_2")
    AddPath(paths,	"Row_3_Ducks_2")
    AddPath(paths,	"Row_Switch_1_1")
    AddPath(paths,	"Row_Switch_1_1")
	AddSpawn(spawn,paths,
		 6920,	1.0,	3.0,	true,	2.0,	8.0,	5.0,	500,		true,	0.5,	1.0,	3.0,	true,	    true,		0.0)

	-- Ducks 8
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Row_1_Ducks_3")
	AddPath(paths,	"Row_2_Ducks_3")
    AddPath(paths,	"Row_3_Ducks_3")
    AddPath(paths,	"Row_Switch_1_2")
    AddPath(paths,	"Row_Switch_1_2")
	AddSpawn(spawn,paths,
		 6919,	1.0,	4.0,	true,	2.0,	8.0,	10.0,	1000,		true,	0.75,	0.5,	4.0,	true,		true,		0.0)
    
    -- Bulls Eye
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_2_5")
	AddPath(paths,	"Mid_Row_1_5")
	AddSpawn(spawn,paths,
		 7801,	5.0,	10.0,	false,	10.0,	15.0,	1.0,	5000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
    -- Bulls Eye 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_2_7")
	AddPath(paths,	"Mid_Row_1_7")
	AddSpawn(spawn,paths,
		 7801,	10.0,	15.0,	false,	10.0,	15.0,	5.0,	5000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
    -- Bulls Eye 3
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_2_6")
	AddPath(paths,	"Mid_Row_1_6")
	AddSpawn(spawn,paths,
		 7801,	15.0,	20.0,	false,	10.0,	15.0,	5.0,	5000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
    -- Bonus
    ----------------------------------------------------------
    paths = {}
	----------------------------------------------------------
    AddPath(paths,	"Upper_1_1")
    AddPath(paths,	"Upper_1_2")
    AddSpawn(spawn,paths,
        5877,	18.0,	20.0,	false,	0,	2,	2.0,	10000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
    
	
	--************************************************************
	--------------------------------------------------------------
    -- Friendly 1
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_1_1")
	AddPath(paths,	"Mid_Row_1_2")
	AddSpawn(spawn,paths,
		 7800,	0.0,	5.0,	false,	10.0,	15.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
    		 
	--------------------------------------------------------------
    -- Friendly 2
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_1_3")
	AddPath(paths,	"Mid_Row_1_4")
	AddSpawn(spawn,paths,
		 7800,	10.0,	15.0,	false,	10.0,	15.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
		 
	--------------------------------------------------------------
    -- Friendly 3
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_2_1")
	AddPath(paths,	"Mid_Row_2_2")
	AddSpawn(spawn,paths,
		 7800,	20.0,	25.0,	false,	10.0,	15.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)	
    
	--------------------------------------------------------------
    -- Friendly 4
	--------------------------------------------------------------
	paths = {}
	----------------------------------------------------------
	AddPath(paths,	"Mid_Row_2_3")
	AddPath(paths,	"Mid_Row_2_4")
	AddSpawn(spawn,paths,
		 7800,	5.0,	12.0,	false,	2.0,	5.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)    
    
	--------------------------------------------------------------
    -- Friendly 5
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Mid_Row_3_1")
	AddPath(paths,	"Mid_Row_3_2")
	AddSpawn(spawn,paths,
		 7800,	12.0,	18.0,	false,	2.0,	4.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)
    
	--------------------------------------------------------------		 
	-- Friendly 6
	----------------------------------------------------------
	paths = {}
	----------------------------------------------------------
	AddPath(paths,	"Mid_Row_3_3")
	AddPath(paths,	"Mid_Row_3_4")
	AddSpawn(spawn,paths,
		 7800,	18.0,	22.0,	false,	2.0,	5.0,	1.0,	-1000,		false,	0.0,	1.0,	1.0,	true,		true,		0.0)

   

    -- Add the spawns for the wave
    AddSpawnsForWave(spawns,spawn)
end

  

----------------------------------------------------------------------------------------------------------------------------
-- Do not Chage -- 
----------------------------------------------------------------------------------------------------------------------------
function targetEscaped(self)
    self:NotifyClientZoneObject{ name= "targetEscaped"}  
    
    local tempScore = m_totalscore + targetEscapedValue
    -- if the current score + the modifier is less than 0 then we aren't going to reduce the score
    if tempScore < 0 then return end
    
    m_totalscore = tempScore
    
	local cannon = getObjectByName("cannonObject")

    cannon:NotifyClientObject{ name = "updateScore", param1 = m_totalscore }
    self:NotifyClientZoneObject{ name= "updatescore", param1= tonumber(m_totalscore)}
end

function setReticule(self)
    local cannon = getObjectByName("cannonObject")    
    --print('setReticule')
    
    cannon:SetShootingGalleryReticuleEffect{inRangeType = "inRange3", cooldownType = "sgCoolDownSmall"}
    self:SetVar("ReticuleSet", true)
end

function onFireEventServerSide(self, msg)
    if msg.args == "CannonStored" and not self:GetVar("ReticuleSet") then
        setReticule(self)
    end
end

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
    	if (msg.name == "FinishedPath") and self:GetVar("WaveStatus") then
    	    if self:GetVar("lastEscapeID") ~= msg.ObjIDSender:GetID() then
                local checkPoints = msg.ObjIDSender:GetActivityPoints{}.points
                
                self:SetVar("lastEscapeID", msg.ObjIDSender:GetID())
                -- check to see if this is not a friendly and remove points from your score
                if (checkPoints > 0) then
                    targetEscaped(self)    
                end
            end
		end
		
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
