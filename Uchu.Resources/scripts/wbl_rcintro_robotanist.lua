require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')

CONSTANTS = {}
CONSTANTS["StrombieFaction"] = 103
CONSTANTS["CorruptedRobotanistFaction"] = 102
CONSTANTS["QuickbuildRobotanistLOT"] = 4928

function onStartup(self) 

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
    Set['tetherRadius']     = 80     -- Tether  Radius
    Set['tetherSpeed']      = 8      -- Tether Speed
    Set['wanderRadius']     = 8      -- Wander Radius
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

    Set['Aggression']     = "Neutral"  -- [Aggressive]--[Neutral]--[Passive]
									   -- [PassiveAggres]-
    Set['AggroNPC']        = false
    Set['AggroDist']      = 3          -- Distance away from target to stop before attacking
    Set['AggroSpeed']     = 2          -- Multiplier of the NPC's base speed to approach while attacking

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
    Set['MovementType']     = "Wander" --["Guard"],["Wander"]
    --**********************************************************************
    -- Attach Way Point Set to NPC -- " this is for NPC's that are not HF placed " 
    Set['WayPointSet']      =  nil
    -- Wander Settings ---------------------------------------------------------
    Set['WanderChance']      = 100          -- Main Weight
    Set['WanderDelayMin']    = 5            -- Min Wander Delay
    Set['WanderDelayMax']    = 5            -- Max Wander Delay
    Set['WanderSpeed']       = 0.5          -- Move speed 
    -- effect 1
    Set['WanderEmote']       = false        -- Enable bool
    Set['WEmote_1']          = 30           -- Weight 
    Set['WEmoteType_1']      = "salute"     -- Animation Type
	

------ Set your Custom ProximityRadius            -----------------------------


	
------ Do not change ----------------------------------------------------------
    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
--------------------------------------------------------------------------------

end

--------------------------------------------------------------------------------
-- onTemplateHit
-- 
-- Notes: Called when onHit occurs in the master template script attached
--        to this 'robotanist'.
--------------------------------------------------------------------------------
--function onTemplateHit(self, msg)

    --local attackerFaction = msg.attacker:GetFaction().faction
    
    -- If it is a Maelstrom Zombie
    --if ( attackerFaction == CONSTANTS["StrombieFaction"] ) then
    
        --print("*****CHANGING FACTION*****")
        --self:SetFaction{ faction = CONSTANTS["CorruptedRobotanistFaction"] }
    
        -- Add one health to the robotanist to account for damage taken from Maelstrom Zombie.
        --local inHealth = self:GetHealth().health
        --self:SetHealth{ health = inHealth + 1 }
    
        -- Attach corrupted effect
        -- What happens if we attach this effect, and then someone in the distance comes closer and
        -- the robotanists ghosts in, will they see the effect?
        --self:PlayFXEffect{ name = "corrupt", effectType = "maelstrom"}
        
        --self:SetVar("AttackingTarget", "NoTarget" )
        --self:SetVar("Aggroed", false)

    --end

--end

--------------------------------------------------------------------------------
-- onTemplateDie
-- 
-- Notes: Called when onDie occurs in the master template script attached
--        to this 'robotanist'.
--------------------------------------------------------------------------------
--function onTemplateDie(self, msg)

    --print("TEMPLATE DIE")
	--local mypos = self:GetPosition().pos
    --local posString = self:CreatePositionString{ x = mypos.x, y = mypos.y, z = mypos.z }.string
	--local myRot = self:GetRotation()
	--local config = { {"rebuild_activators", posString }, {"respawn", 100000 }, {"rebuild_reset_time", -1}, {"no_timed_spawn", true} }
	--RESMGR:LoadObject { objectTemplate = CONSTANTS["QuickbuildRobotanistLOT"], x= mypos.x, y= mypos.y , z= mypos.z, owner = self, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z, configData = config }

--end








