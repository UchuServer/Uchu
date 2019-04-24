-- ================================================
-- L_ACT_CANNON.lua
-- Server Side
-- updated 10/13/10 mrb... friendly ship bug fix
-- ================================================

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('o_ShootingGallery')
require('ai/ACT/L_ACT_GENERIC_ACTIVITY_MGR')
spawns = {}
SPAWN_DATA = {}
CONSTANTS = {}
waves = {}
PLAYER_SCORE = {}
EFFECTS = {}
ACTORS  = {}
LOCALS = {}
local m_totalScore = 0
local g_const = {}

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

function StartLuaNotify(self)
    self:SendLuaNotificationRequest{requestTarget=GAMEOBJ:GetZoneControlID(), messageName="ActivityStateChangeRequest"}
    self:SendLuaNotificationRequest{requestTarget=GAMEOBJ:GetZoneControlID(), messageName="MessageBoxRespond"}
    self:SendLuaNotificationRequest{requestTarget=GAMEOBJ:GetZoneControlID(), messageName="NotifyObject"}
    self:SendLuaNotificationRequest{requestTarget=GAMEOBJ:GetZoneControlID(), messageName="ObjectLoaded"}
end

function StopLuaNotify(self)
    self:SendLuaNotificationCancel{requestTarget=GAMEOBJ:GetZoneControlID(), messageName="ActivityStateChangeRequest"}
    self:SendLuaNotificationCancel{requestTarget=GAMEOBJ:GetZoneControlID(), messageName="MessageBoxRespond"}
    self:SendLuaNotificationCancel{requestTarget=GAMEOBJ:GetZoneControlID(), messageName="NotifyObject"}
    self:SendLuaNotificationCancel{requestTarget=GAMEOBJ:GetZoneControlID(), messageName="ObjectLoaded"}
end

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self)	
	-- set game state
    targetSettings(self)
	resetVars(self)
	LOCALS["GameStarted"] = false
	m_totalScore = 0
	math.randomseed( os.time() )
	
    -- default the instance vars
    self:SetVar("initVelVec",{x = 0, y = 0, z = 0})

    local zoneID = GAMEOBJ:GetZoneControlID()
    
    -- send an object loaded message to the ZoneControl object
    zoneID:ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}
    
    StartLuaNotify(self)

    --------------------------------------------------------------
    -- Constants
    --------------------------------------------------------------
    
    g_const = zoneID:GetVar("CONSTANTS")

    self:SetVar("ImpactSkillID",g_const.IMPACT_SKILLID)
    
    g_const.CANNON_PLAYER_OFFSET = {x = g_const.CANNON_PLAYER_OFFSETx, 
                                    y = g_const.CANNON_PLAYER_OFFSETy, 
                                    z = g_const.CANNON_PLAYER_OFFSETz}
    

    -- muzzle offset
    g_const.CANNON_BARREL_OFFSET = {x = g_const.CANNON_BARREL_OFFSETx,
                                    y = g_const.CANNON_BARREL_OFFSETy,
                                    z = g_const.CANNON_BARREL_OFFSETz}
                                    
    --local startObj = self:GetObjectsInGroup{ignoreSpawners=true,group = "start_pos" }.objects[1]
    
    --if startObj then    
    --    local selfPos = self:GetPosition().pos
    --    local startPos = startObj:GetPosition().pos
    --    local pos = {x = 0, y = 0, z = 0}
        
    --    -- z and x must be switched and negative when sending to the SetShootingGalleryParams
    --    pos.z = -(startPos.x - selfPos.x)
    --    pos.y = startPos.y - selfPos.y
    --    pos.x = -(startPos.z - selfPos.z)
        
    --    g_const["CANNON_PLAYER_OFFSET"] = pos
    --end
    
    self:SetShootingGalleryParams{playerPosOffset =    g_const.CANNON_PLAYER_OFFSET,
                                  projectileVelocity = g_const.CANNON_VELOCITY,
                                  cooldown =           g_const.CANNON_REFIRE_RATE,
                                  muzzlePosOffset =    g_const.CANNON_BARREL_OFFSET,
                                  minDistance =        g_const.CANNON_MIN_DISTANCE,
                                  timeLimit =          g_const.CANNON_TIMEOUT,
                                  cameraFOV =          g_const.CANNON_FOV,
                                  bUseLeaderboards =   g_const.CANNON_USE_LEADERBOARDS }                                        
    
    self:SetVar("TIME_LIMIT", zoneID:GetVar("timelimit"))
    -- for Animations
    self:SetVar("VALID_ACTORS", {3109, 3110, 3111, 3112, 3125, 3126})
    self:SetVar("VALID_EFFECTS", {3122})
    self:SetVar("STREAK_BONUS", {1,2,5,10})
    self:SetVar("Super_Charge_Active", false)
    self:SetVar("Matrix", 1)
    self:SetVar("bInit", true)
end

function onShutDown(self)
    StopLuaNotify(self)
end

function resetVars(self)
	LOCALS["SpawnNum"] = 0
	LOCALS["CurSpawnNum"] = 0
	LOCALS["ThisWave"] = 0
	LOCALS["GameScore"] = 0
	LOCALS["GameTime"] = 0
	LOCALS["GameStarted"] = false
	self:SetVar("m_shotsFired",0)
	self:SetVar("m_maxStreak",0)
	self:SetVar("m_misses",0)
	self:SetVar("m_curStreak",0)
	--self:SetVar("m_targetsHit",0)
	--self:SetVar("m_killsSinceLastShot",0)
    self:SetVar("Current_Super_Charged_Time", 0)
	
	self:SetVar("StreakBonus",0)
	self:SetVar("LastSuperTotal", nil )
	self:SetVar("currentReward", -1)
	self:SetVar("rewards", {-1})
	self:SetVar("Mym_totalScore", 0)
	
    --print('*******************************************')
    --print('stop super charge from resetVars')
    if self:GetVar("bInit") then
        toggleSuperCharge(self, false)
    end
    
    self:SetVar("ImpactSkillID",g_const.IMPACT_SKILLID)
    
	-- reset scores
	if g_const.NUM_WAVES then
        for waveNum = 1, tonumber(g_const.NUM_WAVES) do
            PLAYER_SCORE[waveNum] = 0
        end
    end
    
    ActivityTimerStopAllTimers(self)
end

--------------------------------------------------------------
-- Gets the current activity user or returns nil
--------------------------------------------------------------
function getActivityUser(self)
	local targetID = self:GetActivityUser().userID
	
	if (targetID == 0 or targetID == nil) then
		return nil
	else
		return targetID
	end
end

--------------------------------------------------------------
-- Called after loading a projectile
--------------------------------------------------------------
function onChildLoaded(self, msg)
	-- if we loaded a projectile, fire it
	if msg.templateID == g_const.PROJECTILE_TEMPLATEID then         
		local ballObj = msg.childID
		-- get the skill for the projectile		
		local skill = self:GetVar("ImpactSkillID")

        -- store who the parent is
        self:SetVar("parent", msg.childID)  -- TODO: this should probably be setting the childs parent to self, fix or remove
        
		if (ballObj) and (getActivityUser(self)) and (skill) then		
			-- store values in the projectile
			--ballObj:SetVar("My_Faction", getActivityUser(self):GetFaction().faction)
            storeObjectByName("Player", getActivityUser(self))
			-- set the skill
			ballObj:SetActiveProjectileSkill{ skillID = skill }

			-- store the velocity
			local vec = self:GetVar("initVelVec")
			
			-- set projectile params
			if (vec.x ~= 0 and vec.y ~= 0 and vec.z ~= 0) then
				ballObj:SetProjectileParams{initVel = vec, iProjectileType = 1, fLifeTime = 10.0, owner = self}
			end			
		end		
	end	
end

--------------------------------------------------------------
-- play animations on all actors in the scene, and others
--------------------------------------------------------------
function PlaySceneAnimation(self, anim, bPlayCannon, bPlayPlayer, bMustPlay)

	-- play animation on actors
    local friends = self:GetObjectsInGroup{ group = "cannongroup" }.objects
    local priority = 0.5
    
    if bMustPlay then
        priority = 1.07
    end
    
    for i = 1, table.maxn (friends) do 
        if friends[i] then
            friends[i]:PlayAnimation{ animationID = anim, fPriority = priority }
        end
    end

	-- play on cannon
	if (bPlayCannon == true) then
		self:PlayAnimation{ animationID = anim, fPriority = priority }
	end

	-- play on player
	if (bPlayPlayer == true) then
		local player = getObjectByName("activityPlayer")
		
		player:PlayAnimation{ animationID = anim, fPriority = priority }
	end
end

--------------------------------------------------------------
-- play effects on all effects in the scene
--------------------------------------------------------------
function PlaySceneEffect(effectName)
	-- trigger effects on effect objects
	for k,v in ipairs(EFFECTS) do
		local effect = GAMEOBJ:GetObjectByID(v)
		
		effect:PlayFXEffect{ effectType = effectName }
	end
end

--------------------------------------------------------------
-- Called when the client wants to fire
--------------------------------------------------------------
function onShootingGalleryFire(self, msg)
	-- calculate firing parameters
	local params = self:CalculateFiringParameters{targetPos = msg.targetPos, bUseHighArc = false}	
	local vec = params.outVelVector
	
	if (vec.x ~= 0 and vec.y ~= 0 and vec.z ~= 0) then		
		-- save off the velocity
		self:SetVar("initVelVec",vec)		
		self:SetVar("m_shotsFired", self:GetVar("m_shotsFired") + 1)
		
		local player = getActivityUser(self)
		
		self:PlayFXEffect{effectType = "onfire", ignoreID = player}
 		self:PlayFXEffect{effectType = "onfire2", ignoreID = player}
		player:PlayFXEffect{effectType = "SG-fire", ignoreID = player}
		
		-- Tell the zone control we just fired
		GAMEOBJ:GetZoneControlID():ShootingGalleryFire()		
	end	
	-- play fire animation on actors
	PlaySceneAnimation(self, "fire", false, false)
	PlaySceneEffect("fire")
end

--------------------------------------------------------------
-- Called when getting the cannon's faction
--------------------------------------------------------------
function onGetFaction(self, msg)
	-- return the user's faction
	if (getActivityUser(self)) then
		msg.factionList = getActivityUser(self):GetFaction().factionList
		
		return msg
	end	
end

function toggleSuperCharge(self, enable)	            
    local iSkillID = g_const.CANNON_SKILL
    local iCooldown = g_const.CANNON_REFIRE_RATE
    local bSetChargedState = false
    local meItem1 = self:GetInventoryItemInSlot{slot = 0}.itemID
    local meItem2 = self:GetInventoryItemInSlot{slot = 1}.itemID    
        
    if enable and self:GetVar("Super_Charge_Active") then
        return
	elseif enable and not self:GetVar("Super_Charge_Active") then	
        self:EquipInventory{ itemtoequip = meItem1}
        self:EquipInventory{ itemtoequip = meItem2}
        
	    iSkillID = g_const.CANNON_SUPERCHARGE_SKILL
        iCooldown = 400
        bSetChargedState = true
        --print('enable super charge')        
    else
        self:UnEquipInventory{ itemtounequip = meItem1}
        self:UnEquipInventory{ itemtounequip = meItem2}
        
		self:SetVar("NumberOfCharges", 0 )
        --print('stop super charge')
	end
    
    self:SetShootingGalleryParams{  playerPosOffset		= g_const.CANNON_PLAYER_OFFSET, 
                                    projectileVelocity	= g_const.CANNON_VELOCITY,
                                    cooldown            = iCooldown, 
                                    muzzlePosOffset		= g_const.CANNON_BARREL_OFFSET,
                                    minDistance			= g_const.CANNON_MIN_DISTANCE, 
                                    cameraFOV           = g_const.CANNON_FOV,
                                    bUseLeaderboards    = g_const.CANNON_USE_LEADERBOARDS,
                                    timeLimit           = -1 }
                                        
    self:SetNetworkVar("cbskill", iSkillID)
    self:SetVar("Super_Charge_Active", bSetChargedState)	
    --print("Charge State = " .. tostring(bSetChargedState))
end

function TimerToggle(self, bStart)
    if bStart then
        local wave = GAMEOBJ:GetZoneControlID():GetVar("ThisWave")

        self:SetNetworkVar("count", self:GetVar("TIME_LIMIT"))
        self:SetVar("Started", true)
    else
        -- Stop Timer on Client
        self:SetNetworkVar("Stop", true)		
    end    
end

function pauseChargeCannon(self)
    local superChargeTime = math.ceil(ActivityTimerGetRemainingTime(self, "Super_Charge_Timer"))

    if superChargeTime < 1 and superChargeTime > 0 then
        superChargeTime = 1
    end

    self:SetVar("Super_Charge_Paused", true)
    self:SetVar("Current_Super_Charged_Time", superChargeTime)   

    self:SetNetworkVar("charge_counting", math.ceil(superChargeTime))     
    --print('** Pause Super Charge Timer (time remaining = ' .. superChargeTime .. ') **') 
    ActivityTimerStop(self, "Super_Charge_Timer")
end

function StartChargedCannon(self, optionalTime)
    local iTime = optionalTime
     
    if not iTime then
        iTime = g_const.ChargedTime
    end
    
    self:SetVar("Super_Charge_Paused", false)
    toggleSuperCharge(self, true)               
    ActivityTimerStart(self, "Super_Charge_Timer", 1, iTime)

    if not self:GetVar("WaveStatus") then
        pauseChargeCannon(self)        
    end
end

function intdiv(a,b)
	return math.floor(a/b)
end

function getCurrentBonus(self)
	local streak = self:GetVar("m_curStreak")
	-- Cap the streak
	if (streak > 12) then streak = 12 end
	local res = intdiv(streak, 3)
	
	return res
end

function updateStreak(self)
	local streakBonus = getCurrentBonus(self)
	local curStreak = self:GetVar("m_curStreak")
	local marks = math.mod(curStreak, 3)
	
	self:SetNetworkVar("cStreak", curStreak)
	-- Update the marks
	if curStreak >= 0 and curStreak < 13 then
		-- Mark 1
		if marks == 1  then 
			self:SetNetworkVar("Mark1", true)
		elseif marks == 2  then 
			self:SetNetworkVar("Mark2", true)
		elseif marks == 0 and curStreak > 0  then 
		    self:SetVar("StreakBonus", streakBonus)
			self:SetNetworkVar("ShowStreak", streakBonus + 1)			
			self:SetNetworkVar("Mark3", true)
		else
		    self:SetVar("StreakBonus", streakBonus)
			self:SetNetworkVar("UnMarkAll", true)
		end	
	end		
	
end

-- activity timers ii
----------------------------------------------------------------
-- When ActivityTimerUpdate is sent, basically when a timer hits it updateInterval.
----------------------------------------------------------------
function onActivityTimerUpdate(self, msg)
    --print('** ActivityTimerUpdate: ' .. msg.name)
    if msg.name == "Super_Charge_Timer" and not self:GetVar("Super_Charge_Paused") then
        --print('update super charge timer to: ' .. msg.timeRemaining)
        self:SetNetworkVar("charge_counting", msg.timeRemaining)       
    end 
end

----------------------------------------------------------------
-- When ActivityTimerDone is sent, basically when the activity timer has reached it's duration.
----------------------------------------------------------------
function onActivityTimerDone(self, msg)    
    --print('** ActivityTimerDone: ' .. msg.name)
    if msg.name == "Super_Charge_Timer" and not self:GetVar("Super_Charge_Paused") then
        if self:GetVar("WaveStatus") or self:GetVar("Current_Super_Charged_Time") < 1 then
            --print('super charge timer finished')
            self:SetNetworkVar("charge_counting", 999)            
            self:SetNetworkVar("SuperChargeBar", 0) 
            toggleSuperCharge(self, false)
        end
	elseif (string.starts(msg.name,"SpawnWave")) then -- parse the name to get out the wave number
		if (LOCALS["GameStarted"] == true) then	
            local waveNum = LOCALS["ThisWave"]
            local player = getObjectByName("activityPlayer")
            
            -- store this wave number
			self:SetVar("WaveStatus", true)		
            self:SetVar("ThisWave", waveNum)
            
			if waveNum ~= 1 and self:GetVar("Super_Charge_Paused") then 
                --print('****')
                --print('start super charge from SpawnWave Current time: ' .. self:GetVar("Current_Super_Charged_Time"))      
                StartChargedCannon(self, self:GetVar("Current_Super_Charged_Time"))
                self:SetVar("Current_Super_Charged_Time", 0)
			end
			            
            TimerToggle(self, true)
            
            -- setup spawns for wave
            for k,v in pairs(spawns[tonumber(waveNum)]) do SpawnObject(k,v,self,true) end

            -- there are no more waves left so stop game after this wave
            if (tonumber(waveNum) >= g_const.NUM_WAVES) then
                ActivityTimerStart(self, "GameOver", self:GetVar("TIME_LIMIT"), self:GetVar("TIME_LIMIT"))
            else                
                -- setup next wave
                ActivityTimerStart(self, "EndWave" .. waveNum, self:GetVar("TIME_LIMIT"), self:GetVar("TIME_LIMIT"))
            end
            
            if player then
                self:StartActivityTime{ rerouteID = player, startTime = self:GetVar("TIME_LIMIT") }
                self:ActivityPause{ rerouteID = player, bPause = false }
            end
		end
	elseif (string.starts(msg.name,"EndWave")) then -- called when a wave ends, contains the number of the next wave
        if (LOCALS["GameStarted"] == true) then
            self:SetVar("WaveStatus", false)
            
			-- get rid of current spawns
            TimerToggle(self)
            
			-- record the score
			RecordPlayerWaveScore(self)

			-- get the wave number from the rest of the string
			local waveNum = string.sub(msg.name,8)

			-- store the next wave number
			waveNum = tonumber(waveNum) + 1
			LOCALS["ThisWave"] = waveNum
			
			-- Do whatever we need to during the pause, wave begins after pause
			
			-- play wave animation on actors, cannon, and player
			PlaySceneAnimation(self, "wave" .. LOCALS["ThisWave"], true, true, true)
			-- display wave number to player
			DisplayWaveNumberToPlayer(self, waveNum)

			-- there are no more waves left so stop game after this wave
			if (waveNum > g_const.NUM_WAVES) then
				-- Game Over
                ActivityTimerStart(self, "GameOver", 0.1, 0.1)
			else
				-- setup next wave
				ActivityTimerStart(self, "SpawnWave" .. waveNum, g_const.IN_BETWEEN_WAVE_PAUSE, g_const.IN_BETWEEN_WAVE_PAUSE)
			end

			local player = getObjectByName("activityPlayer")
			
            if player then
                self:ActivityPause{rerouteID = player, bPause = true}
            end
            
            if self:GetVar("Super_Charge_Active") and not self:GetVar("Super_Charge_Paused") then          
                pauseChargeCannon(self)
            end
		end
	elseif (msg.name == "GameOver") then
        -- Send buffer timer here
  	    local player = getObjectByName("activityPlayer")
		local cannonballs = self:GetObjectsInGroup{ group = "cannonball" }.objects
		
        self:ActivityPause{ rerouteID = player, bPause = true }
        
        if #cannonballs >= 1 then
            --print('start endGameBuffer')
            ActivityTimerStart(self, "endGameBuffer", 1, #cannonballs)
        else
            --print('stopGame now')
			RecordPlayerWaveScore(self)
            stopGame(self, false)
		end
		
        TimerToggle(self)
    elseif (string.starts(msg.name,"DoSpawn")) then
        if (LOCALS["GameStarted"] == true) then
            -- get the spawn number from the rest of the string
            local spawnNum = string.sub(msg.name,8)
            -- get the template out of the spawn data
            local SpawnData = SPAWN_DATA[tonumber(spawnNum)]
            local templateID = SpawnData.sdTemplate
            -- get the position of the first waypoint
            local startPos = GAMEOBJ:GetWaypointPos( SpawnData.sdPath, 1 )
            local config = {{"spawndata", SpawnData},{"streakmod", g_const.STREAK_MOD},
                            {"streakbonus", self:GetVar("STREAK_BONUS")}, {"wave", LOCALS["ThisWave"]}, 
                            {"custom_script_server", "scripts/ai/ACT/SG_TARGET.lua" },
                            {"custom_script_client", "scripts/client/ai/SG_TARGET_CLIENT.lua" }}

            -- load the object in the world
            RESMGR:LoadObject { objectTemplate = templateID,
                                bIsSmashable = true,
                                x = startPos.x,
                                y = startPos.y,
                                z = startPos.z,
                                owner = self,
                                configData = config}
        end    
    elseif msg.name == "endGameBuffer" then
        RecordPlayerWaveScore(self)
        stopGame(self, false)
    end 
end

function onActivityNotify(self,msg)	
    local tKills = self:GetVar("CannonBallKills") or {}
    
	-- Don't count any of this if we're firing our super charged cannon ball
	if msg.notification["shot_done"] then				 
		if #tKills < 1 and msg.notification["shot_done"] ~= g_const.CANNON_SUPER_CHARGE then
			self:SetVar("m_curStreak", 0)
			-- This account for real misses
			self:SetVar("m_misses", self:GetVar("m_misses") + 1)
			self:SetNetworkVar("HideStreak", true)
			self:SetNetworkVar("UnMarkAll", true)				
		elseif #tKills > 0 then
            -- registerHit for all of the kills since last shot	                      
            for k,v in ipairs(tKills) do
                registerHit(self, GAMEOBJ:GetObjectByID(v), msg.notification["shot_done"])
            end
            
            if #tKills > 1 then
                self:SetNetworkVar("mHit", true)
            end
		end		
		
        if msg.notification["shot_done"] ~= g_const.CANNON_SUPER_CHARGE then
            if self:GetVar("friendly_hit") then
                self:SetVar("m_curStreak", 0)
            end
            
            updateStreak(self)
        end
        
        self:SetVar("friendly_hit", false)
        self:SetVar("CannonBallKills", {})
	end		
end

function registerHit(self, target, shotType)
	local points = target:GetActivityPoints{}.points
	
	-- Only apply bonus to positive things and don't do streak etc for negative
	if points >= 0 then
		points = points + (points * getCurrentBonus(self))		
		
		if shotType ~= g_const.CANNON_SUPER_CHARGE then
			self:SetVar("m_curStreak", self:GetVar("m_curStreak") + 1)
			
			if (self:GetVar("m_curStreak") > self:GetVar("m_maxStreak")) then
				self:SetVar("m_maxStreak", self:GetVar("m_curStreak"))
			end
		end	
	else
        if shotType ~= g_const.CANNON_SUPER_CHARGE then
            self:SetVar("m_curStreak", 0)
            -- This accounts for friendlies
            self:SetVar("m_misses", self:GetVar("m_misses") + 1)
            self:SetVar("friendly_hit", true)
        end
        
        self:SetNetworkVar("hitFriend", true)
	end

	self:NotifyClientShootingGalleryScore{rerouteID = getActivityUser(self), target = target, targetPos = target:GetPosition().pos, score=points}

    m_totalScore = m_totalScore + points
    
    self:SetNetworkVar("updateScore", tonumber(m_totalScore))
    
    local lastSuperTotal = self:GetVar("LastSuperTotal") or 0
    
    scScore = m_totalScore - lastSuperTotal
    
    if shotType ~= g_const.CANNON_SUPER_CHARGE and scScore >= g_const.ChargedPoints and points >= 0 then
        StartChargedCannon(self)
        self:SetNetworkVar("SuperChargeBar", 100) 
        self:SetVar("LastSuperTotal", m_totalScore)
    end
    
    local rewardS = 0
    local rewardF = 0
    
    if self:GetVar("Matrix") <= 5  then
        local scoreRewardNum = "Score_Reward_"..self:GetVar("Matrix")
        local RewardAmount =  g_const[scoreRewardNum]  / 100 * 3
        
        rewardS = (m_totalScore / RewardAmount)
        rewardF = round((rewardS * 3),0)
        
        if rewardF > 100 then
            rewardF = 100
        end
        
        self:SetNetworkVar("modelPercent", rewardF)
    end

    if rewardF > 0 and rewardF < 200 and self:GetVar("Matrix") <= 5  then
        local obj = self:GetObjectsInGroup{ignoreSpawners=true,group = g_const.Reward_Model_GrpName }.objects --RWS
        
        -- check to make sure obj[1] is not nil, mrb...
        if obj[1] then
            obj[1]:SpawnModelBricks{amount=(rewardF/100),pos=target:GetPosition().pos}

            if rewardF >= 100  then
                --print('giving new model score is: ' .. m_totalScore)
                spawnNewModel(self)
                self:SetVar("Matrix" , self:GetVar("Matrix") + 1 )
            end
        end
    end
    
    self:SetNetworkVar("beatHighScore", tostring(m_totalScore))
    
    checkSpawn(self, target)                  
end

function onUpdateMissionTask(self, msg)
	-- We get these forwarded to us by the cannon ball
	
    if msg.taskType == "kill" then
        local tKills = self:GetVar("CannonBallKills") or {}
        
        table.insert(tKills, "|" .. msg.target:GetID())
        
        self:SetVar("CannonBallKills", tKills)
        
        getActivityUser(self):UpdateMissionTask{target = msg.target, value = msg.value, value2 = msg.value2, taskType = msg.taskType}
    end
end

--------------------------------------------------------------
-- put the player in the cannon, does not start the cannon
--------------------------------------------------------------
function onRequestActivityEnter(self, msg)
    storeObjectByName("activityPlayer", msg.userID)
    self:SetNetworkVar("HideScoreBoard", true)
    self:SetNetworkVar("ReSetSuperCharge", true)
    self:SetNetworkVar("showLoadingUI", true)
    
    -- Preloading reload animations
    -- Cannonballs, plunger, and pirate
    local sceneactors = self:GetObjectsInGroup{ group = "cannongroup" }.objects
    
    for i = 1, table.maxn (sceneactors) do 
        if sceneactors[i] then
            sceneactors[i]:PreloadAnimation{animationID = "wave1", respondObjID = self}
        end
    end
    
    msg.userID:PreloadAnimation{animationID = "wave1", respondObjID = self}    
    self:PreloadAnimation{animationID = "wave1", respondObjID = self}
end
--------------------------------------- from zone

function spawnNewModel(self)
	if self:GetVar("currentReward") ~= -1 then
		local rewards = self:GetVar("rewards")
		
		if rewards and rewards[1] ~= -1 then
			table.insert(rewards, self:GetVar("currentReward"))
		else
			rewards = {self:GetVar("currentReward")}
		end
		
		self:SetVar("rewards",rewards)
        self:SetNetworkVar("rewardAdded", self:GetVar("currentReward"))
	end

	local obj = self:GetObjectsInGroup{ignoreSpawners=true,group= g_const.Reward_Model_GrpName}.objects
	
    if(#obj > 0) then
		for index, rewardObj in pairs(obj) do
			local items = GAMEOBJ:RollLoot(g_const["Score_LootMatrix_"..self:GetVar("Matrix")], getObjectByName("activityPlayer"))

			for LOT,count in pairs(items) do
				rewardObj:SetModelToBuild{templateID=LOT}
				self:SetVar("currentReward", LOT)
			end
		end
	end
end

--------------------------------------------------------------
-- try to start the game
--------------------------------------------------------------
function startGame(self)
	-- clear score
	self:SetNetworkVar("game_timelimit", self:GetVar("TIME_LIMIT"))
	self:SetNetworkVar("Audio_Start_Intro", true)
	
	self:SetVar("currentReward", -1)

	local obj = self:GetObjectsInGroup{ignoreSpawners=true,group= g_const.Reward_Model_GrpName}.objects

	for i, j in pairs(obj) do
		j:SetModelToBuild{templateID=-1}
	end
		
	local idString = self:GetID()
	-- get the player
	local player = getObjectByName("activityPlayer")
	
	if player then
        -- Telkl the client to get the initial data for showing the top score
		local targetID = self:GetActivityUser().userID
		
        targetID:RequestActivitySummaryLeaderboardData{target = self, queryType = 1, gameID = self:GetActivityID().activityID }
	
        -- put the player in if we have to, otherwise start
        if ((self:GetActivityUser().userID):GetID() == player:GetID()) then
            --print("starting game")
            self:ActivityStart{ rerouteID = player }
        end
                
        self:SetNetworkVar("Clear", true)

        DoGameStartup(self)
		
        if not self:GetVar("firstTimeDone") then
            --remove the activity cost from the player as they load into the map
            local takeCost = self:ChargeActivityCost{user = player}.bSucceeded
            --print('cost taken for: ' .. player:GetName().name .. ' = ' .. tostring(takeCost))
        end
        
        self:SetVar("firstTimeDone", true)
    end
    
	spawnNewModel(self)
end

--------------------------------------------------------------
-- try to stop the game
--------------------------------------------------------------
function stopGame(self, bCanceling)
    self:SetNetworkVar("ReSetSuperCharge", true)
    self:SetNetworkVar("HideSuper", true)
    
	--print("STOPING GAME ******************************************")
	-- get the player
	local player = getObjectByName("activityPlayer")

    if not player:Exists() then return end
    
    --print('stop super charge from stopGame()')
    toggleSuperCharge(self, false)    
    
	-- if we have both stop it if we need to, but dont exit
	if not bCanceling then
        -- Now retrieve everything form the cannon. We have the score here so we do the rest here too
        local streak = self:GetVar("m_maxStreak")
        local misses = self:GetVar("m_misses")
        local fired = self:GetVar("m_shotsFired")
        
        self:SetActivityUserData{ userID = player, typeIndex = 0, value = m_totalScore }
        self:SetActivityUserData{ userID = player, typeIndex = 1, value = streak }    
        
        local percentage = 0
        
        if (fired > 0) then
            percentage = (fired-misses) / fired
        end
        
        self:SetActivityUserData{ userID = player, typeIndex = 2, value = percentage }

        -- Tell the leaderboard to store everything
        self:UpdateActivityLeaderboard{ userID = player }
        
        self:SetVar("Mym_totalScore", m_totalScore)
        m_totalScore= 0
        
        LOCALS["GameStarted"] = false
        
        showSummaryDialog(self, fired, fired-misses, streak)		
        awardModels(self:GetVar("rewards"), player)	
    end
    
    self:ActivityStop{rerouteID = player, bExit = false, bUserCanceled = bCanceling}    
    DoGameShutdown(self)    
    resetVars(self)
end

function awardModels(rewards, player)
	if not rewards then return end
	
    for index, reward in pairs(rewards) do
        if reward ~= -1 then
            player:AddItemToInventory{iObjTemplate = reward, invType = 5 --[[5 is INVENTORY_MODEL--]]  }
        end
    end
end

--------------------------------------------------------------
-- sends a message to display the current wave text to player
--------------------------------------------------------------
function DisplayWaveNumberToPlayer(self, waveNum)
	local player = getObjectByName("activityPlayer")
	
	if (player) then		
        self:SetNetworkVar("wave", {waveNum = waveNum, waveStr = self:GetVar("TIME_LIMIT")})
	end
end

--------------------------------------------------------------
-- show the summary dialog
--------------------------------------------------------------
function showSummaryDialog(self, shots, kills, streak)
	-- get player
	local player = getObjectByName("activityPlayer")

	if player then -- do summary dialog
		-- Wave Score
		local score = m_totalScore
		local r = self:GetActivityReward{ playerID = player}
		local rstring = (r.rewardMoney..","..r.rewardItem1Name..","..r.rewardItem1Image..","..r.rewardItem1StackSize..","..r.rewardItem2Name..","..r.rewardItem2Image..","..r.rewardItem2StackSize)
        local rscore = ( self:GetVar("Mym_totalScore").."_"..PLAYER_SCORE[1].."_"..PLAYER_SCORE[2].."_"..PLAYER_SCORE[3].."_"..shots.."_"..kills.."_"..streak )

		-- Send the activity ID down to the client
        self:SetNetworkVar("UI_Rewards", tostring(rscore)) --self:GetActivityID().activityID)
        self:SetNetworkVar("Audio_Final_Wave_Done", true)

		local targetID = self:GetActivityUser().userID
		
        targetID:RequestActivitySummaryLeaderboardData{target = self, queryType = 1, gameID = self:GetActivityID().activityID }
	end
end

function RecordPlayerWaveScore(self)
	-- get wave number
	local waveNum = tonumber(LOCALS["ThisWave"])

	if (waveNum > 0) then
		-- get total current score
		local score = m_totalScore

		-- to get the wave score we must subtract prior waves from it
		for waves = 1, waveNum - 1 do
			score = score - PLAYER_SCORE[waves]
		end

		-- store the new score
		PLAYER_SCORE[waveNum] = score
        --print("*************** SCORE? "..PLAYER_SCORE[waveNum])
	end
end

--------------------------------------------------------------
-- Sent from the cannon to get a score for the player
--------------------------------------------------------------
function onDoCalculateActivityRating(self, msg)
    -- also return the score as the result for the activity
	msg.outActivityRating = self:GetVar("Mym_totalScore")

	return msg
end

function checkSpawn(self, target)
	local waveNum = tonumber(LOCALS["ThisWave"])
	if (LOCALS["GameStarted"] == true) and (waveNum) and (waveNum > 0) then
		local spawnData = target:GetVar("SpawnData")

		-- spawn the right object
		if (spawnData) and (spawnData.sdRespawn == true)  and (target:GetVar("wave") == LOCALS["ThisWave"]) then
		    spawns[waveNum][spawnData.sdnum].sdLastPath = spawnData.sdPath
			SpawnObject(spawnData.sdnum,spawns[waveNum][spawnData.sdnum],self,false)
		end
	end
end

function RemovePlayerFromZone(self, player)
    if not player then return end

	player:TransferToLastNonInstance{ playerID = player, bUseLastPosition = true }
end

--------------------------------------------------------------
-- User is exiting via cancel
--------------------------------------------------------------
function onRequestActivityExit(self, msg)
	if (msg.bUserCancel == true) then
		stopGame(self,msg.bUserCancel)
		RemovePlayerFromZone(self, msg.userID)
        --self:SetNetworkVar("HideScoreBoard", true) 
	end
end

--------------------------------------------------------------
-- handle all the game startup data
--------------------------------------------------------------
function DoGameStartup(self)
	-- set game state and vars
	resetVars(self)
	LOCALS["GameStarted"] = true
    self:SetNetworkVar("Clear", true)

	-- start the first wave
	LOCALS["ThisWave"] = 1
	
	if g_const.FIRST_WAVE_START_TIME < 1 then
	     g_const.FIRST_WAVE_START_TIME = 1
	end
	
    ActivityTimerStart(self, "SpawnWave1", g_const.FIRST_WAVE_START_TIME, g_const.FIRST_WAVE_START_TIME)
end

-----------------------------------------------------------------
-- Target Spawning logic
-----------------------------------------------------------------

--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)
	-- look through spawn data to find the most recent data
	-- that matches the template
	local SpawnData = GetLatestSpawnDataByTemplate(self,msg.templateID)

	if (SpawnData) then
		local curSpawnNum = IncrementVarAndReturn("CurSpawnNum")

		-- store spawn for use later
		storeObjectByName("spawnObject" .. curSpawnNum, msg.childID)
		-- store who the parent is
		storeParent(self, msg.childID)
		-- store the spawn data in the child (@TODO: Need to Optimize)
		msg.childID:SetVar("SpawnData", SpawnData)

		if (SpawnData.sdMovingPlat == true) then
			msg.childID:SetPathingSpeed{ speed = SpawnData.sdSpeed }
			msg.childID:SetCurrentPath{ pathName = SpawnData.sdPath, startPoint = 0 }
		else
			-- assign child's waypoint
			msg.childID:SetVar("attached_path",SpawnData.sdPath)
			msg.childID:SetVar("attached_path_start",0)
			
			local startpos = GAMEOBJ:GetWaypointPos( SpawnData.sdPath, 1 )
			
			msg.childID:SetPosition{pos = startpos}
			-- start child on path
			msg.childID:FollowWaypoints()
			msg.childID:SetPathingSpeed{ speed = SpawnData.sdSpeed }
		end
	end
end

--------------------------------------------------------------
-- return if template is a valid actor
--------------------------------------------------------------
function IsValidActor(templateID)
	for k,v in ipairs(self:GetVar("VALID_ACTORS")) do
		if templateID == v then
			return true
		end
	end

	return false
end

--------------------------------------------------------------
-- return if template is a valid effect
--------------------------------------------------------------
function IsValidEffect(templateID)
	for k,v in ipairs(self:GetVar("VALID_EFFECTS")) do
		if templateID == v then
			return true
		end
	end

	return false
end

--------------------------------------------------------------
-- handle all the game shutdown data
--------------------------------------------------------------
function DoGameShutdown(self)
	----print("DoGameShutdown GAME ******************************************")
	LOCALS["GameStarted"] = false

	-- cancel all timers
	ActivityTimerStopAllTimers(self)

	-- despawn all spawns
	DestroyAllSpawns(self)
end

--------------------------------------------------------------
-- Get a random path
--------------------------------------------------------------
function GetRandomPath(self, spawn, lastPath)
	-- pick a random
	local newPath = lastPath
    local lastRandomPath = self:GetVar("lastRandomPath") or ""
    
    if not lastPath then 
        lastPath = "" 
    end
	
	if #spawn.path > 1 then
	    local newPathTable = {}
	    
	    for k,v in ipairs(spawn.path) do
	        -- if we find the lastPath or lastRandomPath then remove it from the temp table
            if v ~= lastPath or v ~= lastRandomPath then
	            table.insert(newPathTable, v)
	        end
	    end
	    
	    -- if we still have paths to pick from then get a random path 
	    if newPathTable then	    
            --print('** multi Path ' .. #newPathTable )
            newPath = spawn.path[math.random(1,#newPathTable)]
        else
            -- otherwise use the first path
            newPath = false
        end
    else
        --print('** one Path')
        if spawn.path[1] == lastRandomPath then
            newPath = false
        else
            newPath = spawn.path[1]
        end
    end
    
    --print('spawn: ' .. spawn.id .. ' @ ' .. tostring(newPath) .. ' : lastPath = ' .. lastPath)-- .. ' : lastRandPath = ' .. lastRandomPath)
    self:SetVar("lastRandomPath", newPath)
    
	return newPath
end

--------------------------------------------------------------
-- spawn an object for the game
--------------------------------------------------------------
function SpawnObject(num, spawn, self, bSpawnNow)
	-- get the current spawn number
	local spawnNum = IncrementVarAndReturn("SpawnNum")
	local newPath = GetRandomPath(self, spawn, spawn.sdLastPath)
	
	if not newPath then 
        spawn.bRespawn = false 
    end
	
	-- save the spawn data for the object when it is loaded
	local SpawnData = { sdTemplate = spawn.id,
                        sdRespawn = spawn.bRespawn,
                        sdSpeed = spawn.speed,
                        sdScore = spawn.score,
                        sdPath = newPath,
                        sdnum = num,
                        sdChangeSpeed = spawn.bChangeSpeed,
                        sdSpeedChance = spawn.speedChangeChance,
                        sdMinSpeed = spawn.minSpeed,
                        sdMaxSpeed = spawn.maxSpeed,
                        sdMovingPlat = spawn.bMovingPlatform,
                        sdDespawnTime = spawn.despawnTime,
                        sdTimeScore = spawn.timeScore,
                        bSpawned = false}

	-- store the data
	SPAWN_DATA[tonumber(spawnNum)] = SpawnData

	-- set the timer to spawn the object
	local timerName = "DoSpawn" .. spawnNum

	-- spawn now and use initial spawn times
	if (bSpawnNow == true) then
		if (spawn.initSpawnTimeMin > 0 and spawn.initSpawnTimeMax > 0) then
			local ranSpawnTime = (math.random() * (spawn.initSpawnTimeMax - spawn.initSpawnTimeMin)) + spawn.initSpawnTimeMin
			
            --print ("init spawning with time " .. ranSpawnTime)
			if ranSpawnTime < 1 then
			    ranSpawnTime = 1
			end
			
			ActivityTimerStart(self, timerName, ranSpawnTime, ranSpawnTime)
		else
			ActivityTimerStart(self, timerName, 1, 1)
		end	
	elseif (spawn.bRespawn == true) then -- respawn, use respawn times
		-- pick a random spawn time
		local ranSpawnTime = (math.random() * (spawn.maxTime - spawn.minTime)) + spawn.minTime
		
        if ranSpawnTime < 1 then
            ranSpawnTime = 1
        end
        
        ActivityTimerStart(self, timerName, ranSpawnTime, ranSpawnTime)
	end
end

--------------------------------------------------------------
-- destroys all spawns
--------------------------------------------------------------
function DestroyAllSpawns(self)
	local maxSpawnNum = LOCALS["CurSpawnNum"]
	
	for spawn = 1, maxSpawnNum do
		local spawnObject = getObjectByName("spawnObject" .. spawn)
		
		if (spawnObject) then
			if (spawnObject:Exists() and not spawnObject:IsDead().bDead) then
				--print("removing spawn object " .. spawn)
				spawnObject:Die{killerID = spawnObject, killType = "SILENT"}
			end
		end
	end

	-- reset vars
	LOCALS["SpawnNum"] = 0
	LOCALS["CurSpawnNum"] = 0
end

--------------------------------------------------------------
-- look through spawn data to find the most recent data
-- that matches the template, returns nil or data
--------------------------------------------------------------
function GetLatestSpawnDataByTemplate(self,templateID)
	local spawnNum = LOCALS["SpawnNum"]
	
	while (spawnNum > 0) do
		-- get the data
		local SpawnData = SPAWN_DATA[tonumber(spawnNum)]

		-- check spawn flag and template
		if (SpawnData.bSpawned == false and templateID == SpawnData.sdTemplate) then
			-- set spawned flag
			SpawnData.bSpawned = true
			-- re-save data
			SPAWN_DATA[tonumber(spawnNum)] = SpawnData
			-- return the good data
			return SpawnData
		end

		-- try prev spawn data
		spawnNum = spawnNum - 1
	end

	return nil
end 

--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function notifyMessageBoxRespond(self, other, msg)
    if other:GetID() ~= GAMEOBJ:GetZoneControlID():GetID() then return end
    
    --print(msg.identifier .. " - " .. msg.iButton)
	-- make sure this is the right player
	local player = getObjectByName("activityPlayer")
	
	if not player then return end
	
	if (player:GetID() == msg.sender:GetID()) then
		if msg.identifier == "Scoreboardinfo" then
			strText = "Retry?"
			-- show the summary message box
			player:DisplayMessageBox{bShow = true,
									 imageID = 2,
									 callbackClient = GAMEOBJ:GetZoneControlID(),
									 text = strText,
									 identifier = "Shooting_Gallery_Retry"}
		else
			-- User wants to retry or is closing the big help to start the game
			if ((msg.iButton == 1 and (msg.identifier == "Shooting_Gallery_Retry" or 
                    msg.identifier == "RePlay")) or (msg.identifier == "SG1") or 
                    msg.iButton == 0) then
                self:SetNetworkVar("Clear", true)
				startGame(self)			
			elseif (msg.iButton == 0 and (msg.identifier == "Shooting_Gallery_Retry" or 
                    msg.identifier == "RePlay")) then -- User wants to quit
				-- called before we leave the cannon, so we can trigger a loading screen
				-- as soon as possible. Should still remove player from cannon on server
                self:RequestActivityExit{userID = player, bUserCancel = false}
				RemovePlayerFromZone(self, player)
			elseif msg.iButton == 1 and msg.identifier == "Shooting_Gallery_Exit" then
                RemovePlayerFromZone(self, player)
                self:RequestActivityExit{userID = player, bUserCancel = true}
			end
		end
	end
end

function notifyObjectLoaded(self, other, msg)
    if other:GetID() ~= GAMEOBJ:GetZoneControlID():GetID() then return end
    
    if ( IsValidActor(msg.templateID) == true ) then
		-- store the actor
		local nextActor = #ACTORS + 1
		
		ACTORS[nextActor] = msg.objectID:GetID()	
	elseif ( IsValidEffect(msg.templateID) == true ) then -- check for effects
		-- store the actor
		local nextEffect = #EFFECTS + 1
		
		EFFECTS[nextEffect] = msg.objectID:GetID()
	end
end

function onNotifyObject(self, msg)    
    if (msg.name == "FinishedPath") then
		checkSpawn(self, msg.ObjIDSender)
	end
end

function notifyActivityStateChangeRequest(self,other,msg)
    if other:GetID() ~= GAMEOBJ:GetZoneControlID():GetID() then return end
    
    if (msg.iNumValue1 == 1000) then -- retry
        startGame(self)
	elseif (msg.iNumValue1 == 7777) then -- qurriey
		local targetID = self:GetActivityUser().userID
		
        targetID:RequestActivitySummaryLeaderboardData{target = self, queryType = 1, gameID = self:GetActivityID().activityID }
	elseif (msg.iNumValue1 == 2000) then -- exit
		local player = getObjectByName("activityPlayer")

        strText = "Exit?"
        -- show the summary message box
        player:DisplayMessageBox{   bShow = true,
                                    imageID = 1,
                                    callbackClient = GAMEOBJ:GetZoneControlID(),
                                    text = strText,
                                    identifier = "Shooting_Gallery_Exit"}
	elseif (msg.iNumValue1 == 1200) then -- play
        startGame(self)	
	end
end
