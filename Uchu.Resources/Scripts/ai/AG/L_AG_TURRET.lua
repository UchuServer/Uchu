require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')




CONSTANTS = {}
CONSTANTS["RebuildStateOpen"] = 0
CONSTANTS["RebuildStateCompleted"] = 2
CONSTANTS["DarklingMechLOT"] = 6253
CONSTANTS["QuickbuildTurretLOT"] = 6254
CONSTANTS["TickTime"] = 1
CONSTANTS["KillTime"] = 20

function onStartup(self) 
Set = {}

--New AI Override
------------------------------------
Set['SuspendLuaAI']          = true      -- a state suspending scripted AI
----------------------------------------

--[[
///////////////////////////////////////////////////////////////////////////
         ____    _  __  ___   _       _       ____  
        / ___|  | |/ / |_ _| | |     | |     / ___| 
        \___ \  | ' /   | |  | |     | |     \___ \ 
         ___) | | . \   | |  | |___  | |___   ___) |
        |____/  |_|\_\ |___| |_____| |_____| |____/                                                                                     
--]]

  
    Set['OverRideHealth']   = false   -- Bool Health Overide
    Set['Health']           = 1       -- Amount of health

    Set['OverRideImag']     = false   -- Bool Imagination Overide
    Set['Imagination']      = nil     -- Amout of Imagination

    Set['OverRideImmunity'] = false   -- Bool Immunity Overide
    Set['Immunity']         = false   -- Bool
    
    Set['OverRideName']     = false
    Set['Name']             = "Master Template" 

    Set['EmoteReact']       = false
    Set['Emote_Delay']      = 2
    Set['React_Set']        = "test"
	
--[[
///////////////////////////////////////////////////////////////////////////
         ____       _      ____    ___   _   _   ____  
        |  _ \     / \    |  _ \  |_ _| | | | | / ___| 
        | |_) |   / _ \   | | | |  | |  | | | | \___ \ 
        |  _ <   / ___ \  | |_| |  | |  | |_| |  ___) |
        |_| \_\ /_/   \_\ |____/  |___|  \___/  |____/         
--]]

    Set['aggroRadius']      = 30     -- Aggro Radius
    Set['conductRadius']    = 15     -- Conduct Radius
    Set['tetherRadius']     = 30     -- Tether  Radius
    Set['tetherSpeed']      = 8      -- Tether Speed
    Set['wanderRadius']     = 0      -- Wander Radius
    --- FOV Radius -- 
    -- Aggro
    Set['UseAggroFOV']      = false
    Set['aggroFOV']         = 180 
    -- Conduct
    Set['UseConductFOV']    = false
    Set['conductFOV']       = 180 
--[[
////////////////////////////////////////////////////////////////////////////////
            _       ____    ____   ____     ___  
           / \     / ___|  / ___| |  _ \   / _ \ 
          / _ \   | |  _  | |  _  | |_) | | | | |
         / ___ \  | |_| | | |_| | |  _ <  | |_| |
        /_/   \_\  \____|  \____| |_| \_\  \___/       
        
--]] 

    Set['Aggression']     = "Aggressive"  -- [Aggressive]--[Neutral]--[Passive]
									   -- [PassiveAggres]-
    Set['AggroNPC']        = false
    Set['AggroDist']      = 20          -- Distance away from target to stop before attacking
    Set['AggroSpeed']     = 0          -- Multiplier of the NPC's base speed to approach while attacking

    -- Aggro Emote
    Set['AggroEmote']      = false     --Plays Emote on Aggro 
    Set['AggroE_Type']     = ""        -- String Name of Emote
    Set['AggroE_Delay']    = 1         -- Animation Delay Time
    

--[[
///////////////////////////////////////////////////////////////////////////
         __  __    ___   __     __  _____   __  __   _____   _   _   _____ 
        |  \/  |  / _ \  \ \   / / | ____| |  \/  | | ____| | \ | | |_   _|
        | |\/| | | | | |  \ \ / /  |  _|   | |\/| | |  _|   |  \| |   | |  
        | |  | | | |_| |   \ V /   | |___  | |  | | | |___  | |\  |   | |  
        |_|  |_|  \___/     \_/    |_____| |_|  |_| |_____| |_| \_|   |_|  
--]]

    --**********************************************************************
    Set['MovementType']     = "Guard" --["Guard"],["Wander"]
    --**********************************************************************
    -- Attach Way Point Set to NPC -- " this is for NPC's that are not HF placed " 
    Set['WayPointSet']      =  nil
    -- Wander Settings ---------------------------------------------------------
    Set['WanderChance']      = 0            -- Main Weight
    Set['WanderDelayMin']    = 5            -- Min Wander Delay
    Set['WanderDelayMax']    = 5            -- Max Wander Delay
    Set['WanderSpeed']       = 0            -- Move speed 
    -- effect 1
    Set['WanderEmote']       = false        -- Enable bool
    Set['WEmote_1']          = 30           -- Weight 
    Set['WEmoteType_1']      = "salute"     -- Animation Type
	
	
------ Do not change ----------------------------------------------------------
    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
--------------------------------------------------------------------------------


	----------------------------------
	-- Sets the turret's life timer and disables its AI which will only be active after rebuild.
	self:SetVar("TickTime", 1)
	self:SetVar("currentTime", 1)
	--disable AI here:
	self:EnableCombatAIComponent{bEnable = false}
	GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
	----------------------------------*
	self:SetVar("building", 0)
	self:SetGravityScale{scale = 0.0}
	self:SetStunImmunity{StateChangeType = "PUSH", bImmuneToStunAttack = true, bImmuneToInterrupt = true} -- Make immune to stuns
	self:SetStatusImmunity{ StateChangeType = "PUSH", bImmuneToPullToPoint = true, bImmuneToKnockback = true } -- Make immune to move/teleport behaviors
end

--------------------------------------------------------------------------------
-- onRebuildNotifyState
-- 
-- Notes: Whenever the rebuild state changes on the robotanist, this is called.
--------------------------------------------------------------------------------
function onRebuildNotifyState(self, msg)
    
	if (msg.iState == CONSTANTS["RebuildStateCompleted"]) then

	    -- Enable AI here:
	    self:EnableCombatAIComponent{bEnable = true}
	    self:SetVar("building", 0)
	    self:SetParentObj{objIDParent = msg.player}
	    GAMEOBJ:GetTimer():CancelAllTimers( self )
	    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
	end
	
	if (msg.iState == CONSTANTS["RebuildStateOpen"]) then
			
	end
	
end

-------------------------
-- Tells the turret to die after the timer runs out.
-------------------------
onTimerDone = function(self, msg)
    -- 1 second tick timer to increment an overall time (currentTime)
    if msg.name == "TickTime" then
       self:SetVar("currentTime", (self:GetVar("currentTime") + 1))
       --  When the currentTime exceeds 49 seconds and the player is not currently building, kill the turret
       if (self:GetVar("currentTime") >= CONSTANTS["KillTime"]) and (self:GetVar("building") == 0)then
           self:EnableCombatAIComponent{bEnable = false}
            self:Die()
            self:SetVar("currentTime", 0)
       end
       GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
    end
end

-----------------------------
-- Getting the playerID of the rebuilder and setting new timer for the life of the turret
-----------------------------
function onRebuildStart(self, msg)
	self:LockNodeRotation{nodeName = "base"}
    storeObjectByName (self, "playerID", msg.userID )
   -- GAMEOBJ:GetTimer():CancelAllTimers( self )
    self:SetVar("building", 1)
   
end
-----------------------------
-- Reroute Kill credit to the player
-----------------------------
function onUpdateMissionTask(self,msg)

	if msg then
		local player = getObjectByName(self, "playerID")
		if player then
			player:UpdateMissionTask{target = msg.target, value = msg.value, value2 = msg.value2, taskType = msg.taskType}
		end
	end
end






----------------------------------
-- Ensuring the turret is spawned in if the player cancels the rebuild.
---------------------------------
--[[
function onRebuildCancel(self, msg)
	        self:EnableCombatAIComponent{bEnable = false}
            self:SetVar("building", 0)
            if (self:GetVar("currentTime") < 21) then
               local mypos = self:GetPosition().pos
               local posString = self:CreatePositionString{ x = mypos.x, y = mypos.y, z = mypos.z }.string
	           local myRot = self:GetRotation()
               local config = { {"rebuild_activators", posString }, {"respawn", 100000 }, {"rebuild_reset_time", -1}, {"no_timed_spawn", true}, {"currentTime", self:GetVar("currentTime")} }--, {"CheckPrecondition" , "0:21"} }
	           RESMGR:LoadObject { objectTemplate = CONSTANTS["QuickbuildTurretLOT"], x= mypos.x, y= mypos.y , z= mypos.z, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z, configData = config, owner = parent }
    	    end
end
--]]