--------------------------------------------------------------
-- Server side script for Broombot
--
-- Handles User interaction for cleaning or rebuilding
-- Handles state during rebuild
--------------------------------------------------------------


--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')
require('c_Zorillo')


--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self) 

    -- var for bot in use by player for cleaning
    self:SetVar("Bot_InUse", false )
    
Set = {}
    
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
    Set['Name']             = "Broombot"

    Set['EmoteReact']       = false
    Set['Emote_Delay']      = 2
    Set['React_Set']        = "test"
    
    Set['OverRideAttackSkill']       = true     -- Bool Attack Skill Override
    Set['AttackSkill']               = 115      -- Skill to use when Attacking
    Set['UseOptionalTargetOnAttack'] = true     -- Pass target for skill cast on attack
	
--[[
///////////////////////////////////////////////////////////////////////////
         ____       _      ____    ___   _   _   ____  
        |  _ \     / \    |  _ \  |_ _| | | | | / ___| 
        | |_) |   / _ \   | | | |  | |  | | | | \___ \ 
        |  _ <   / ___ \  | |_| |  | |  | |_| |  ___) |
        |_| \_\ /_/   \_\ |____/  |___|  \___/  |____/         
--]]
    Set['aggroRadius']      = 30     -- Aggro Radius
    Set['conductRadius']    = 1     -- Conduct Radius
    Set['tetherRadius']     = 40     -- Tether  Radius
    Set['tetherSpeed']      = 1      -- Tether Speed
    Set['wanderRadius']     = 1      -- Wander Radius
    --- FOV Radius -- 
    -- Aggro
    Set['UseAggroFOV']      = false
    Set['aggroFOV']         = 180 
    -- Conduct
    Set['UseConductFOV']    = false
    Set['conductFOV']       = 180 
	
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
    Set['WanderChance']      = 100          -- Main Weight
    Set['WanderDelayMin']    = 2            -- Min Wander Delay
    Set['WanderDelayMax']    = 5            -- Max Wander Delay
    Set['WanderSpeed']       = .75            -- Move speed 
    -- effect 1
    Set['WanderEmote']       = false        -- Enable bool
    Set['WEmote_1']          = 30           -- Weight 
    Set['WEmoteType_1']      = "salute"     -- Animation Type
	
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
    Set['AggroDist']      = 8          -- Distance away from target to stop before attacking
    Set['AggroSpeed']     = 3          -- Multiplier of the NPC's base speed to approach while attacking

    -- Aggro Emote
    Set['AggroEmote']      = false     --Plays Emote on Aggro 
    Set['AggroE_Type']     = ""        -- String Name of Emote
    Set['AggroE_Delay']    = 1         -- Animation Delay Time
    
	
 
------ Do not change ----------------------------------------------------------
    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
end


--------------------------------------------------------------
-- Returns true if the object is in the idle rebuild state
--------------------------------------------------------------
function IsActive(self)

    -- get the rebuild state
    local rebuildState = self:GetRebuildState()
    
    -- if the state is idle we are active
    if (rebuildState and tonumber(rebuildState.iState) == 3) then
        return true
    else
        return false
    end

end


--------------------------------------------------------------
-- Sets ourself busy and cleans a target
--------------------------------------------------------------
function CleanTarget(self, myTarget)

    -- disable us
    setState("AiDisable",self)
    
    -- set in use
    self:SetVar("Bot_InUse", true )

    -- This replaces StopMoving, which wont work when an object is pathing.
    self:FaceTarget{ target = myTarget, degreesOff = 5, keepFacingTarget = true }
    
    self:CastSkill{optionalTargetID = myTarget, skillID = 115 }  
    
    local anim_time = self:GetAnimationTime{animationID = "destink"}.time
    GAMEOBJ:GetTimer():AddTimerWithCancel( anim_time  , "BackToWork", self )

end


--------------------------------------------------------------
-- Handle client clicking on the object on server
--------------------------------------------------------------
function onUse(self, msg) 

    local rebuildState = self:GetRebuildState()
    
    -- if the state is idle we are active and not in use
    if (IsActive(self) == true and self:GetVar("Bot_InUse") == false) then

        -- clean the player
        CleanTarget(self,msg.user)
    
        --self:RebuildReset()
      
    end
    
end


--------------------------------------------------------------
-- Called when a rebuild is reset
--------------------------------------------------------------
function onRebuildReset(self, msg)

    GAMEOBJ:GetTimer():CancelAllTimers( self )

    -- disable us
    setState("AiDisable",self)
  
end


--------------------------------------------------------------
-- Handle notification of rebuild changes
--------------------------------------------------------------
function onRebuildNotifyState(self, msg)

    GAMEOBJ:GetTimer():CancelAllTimers( self )

    -- if we hit the complete state we are now working for a player    
    if (msg.iState == 2 and msg.player:GetID() ~= CONSTANTS["NO_OBJECT"]) then

	    --print("i am now working for " .. msg.player:GetID())
	    
	    -- record the player who built it
	    storeObjectByName(self, "playerBuilder", msg.player)
	    
	    GAMEOBJ:GetZoneControlID():NotifyObject{ name="broombot_fixed", ObjIDSender = msg.player }

    -- if we just hit the idle state
	elseif (msg.iState == 3) then
	
	    
	    -- get back to work after after we are repaired
	    GAMEOBJ:GetTimer():AddTimerWithCancel( 4.0 , "BackToWork", self )
	    
	end
	
end   


--------------------------------------------------------------
-- Timer notifications
--------------------------------------------------------------
function onTemplateTimerDone(self, msg)

    if (msg.name == "BackToWork") then

        GAMEOBJ:GetTimer():CancelAllTimers( self )
        
        -- disable us
        setState("AiEnable",self)
        
        -- no longer in use
        self:SetVar("Bot_InUse", false )
        
    elseif (msg.name == "Breakdown") then
    
        GAMEOBJ:GetTimer():CancelAllTimers( self )

        -- break down
        setState("AiDisable",self)
        self:RebuildReset()        
    
    end

end



