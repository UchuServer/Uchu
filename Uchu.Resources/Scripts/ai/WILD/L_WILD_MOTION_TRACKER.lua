require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main_Motion_Tracker')

function onStartup(self)

    -- if you really want to do this, do it in the handling of PhysicsComponentReady, not here
	--self:OverrideFriction{bEnableOverride = true, fFriction = 40}
    self:AddObjectToGroup{ group = "motiontracker" }
    local placeposx = self:GetPosition{}.pos.x
    local placeposy = self:GetPosition{}.pos.y
    local placeposz = self:GetPosition{}.pos.z

--    RESMGR:LoadObject {objectTemplate = 6625, x = placeposx, y = placeposy, z = placeposz - 5, owner = self}

Set = {}

--[[
///////////////////////////////////////////////////////////////////////////
         ____       _      ____    ___   _   _   ____  
        |  _ \     / \    |  _ \  |_ _| | | | | / ___| 
        | |_) |   / _ \   | | | |  | |  | | | | \___ \ 
        |  _ <   / ___ \  | |_| |  | |  | |_| |  ___) |
        |_| \_\ /_/   \_\ |____/  |___|  \___/  |____/ 
--]]

    Set['aggroRadius']      = 50     -- Aggro Radius
    Set['conductRadius']    = 50     -- Conduct Radius
    Set['tetherRadius']     = 100     -- Tether  Radius
    Set['tetherSpeed']      = 1      -- Tether Speed
    Set['wanderRadius']     = 15      -- Wander Radius
    -- Aggro
    Set['UseAggroFOV']      = false
    Set['aggroFOV']         = 10
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

    Set['Aggression']      = "Aggressive"  -- [Aggressive] [Neutral] [Passive] [PassiveAggres]
    Set['AggroNPC']        = false
    Set['AggroDist']       = 6             -- Distance away from target to stop before attacking
    Set['AggroSpeed']      = 2             -- Multiplier of the NPC's base speed to approach while attacking

    -- Aggro Emote
    Set['AggroEmote']      = false          -- Plays Emote on Aggro 
    Set['AggroE_Type']     = ""            -- String Name of Emote
    Set['AggroE_Delay']    = 1             -- Animation Delay Time

--[[
///////////////////////////////////////////////////////////////////////////
         __  __    ___   __     __  _____   __  __   _____   _   _   _____ 
        |  \/  |  / _ \  \ \   / / | ____| |  \/  | | ____| | \ | | |_   _|
        | |\/| | | | | |  \ \ / /  |  _|   | |\/| | |  _|   |  \| |   | |  
        | |  | | | |_| |   \ V /   | |___  | |  | | | |___  | |\  |   | |  
        |_|  |_|  \___/     \_/    |_____| |_|  |_| |_____| |_| \_|   |_|  
--]]

    --**********************************************************************
    Set['MovementType']     = "Wander" -- ["Guard"],["Wander"]
    --**********************************************************************
    -- Attach Way Point Set to NPC -- " this is for NPC's that are not HF placed " 
    Set['WayPointSet']      =  nil
    -- Wander Settings ---------------------------------------------------------
    Set['WanderChance']      = 1        -- Main Weight
    Set['WanderDelayMin']    = 1          -- Min Wander Delay
    Set['WanderDelayMax']    = 1          -- Max Wander Delay
    Set['WanderSpeed']       = 2          -- Move speed 
    -- effect 1
    Set['WanderEmote']       = false      -- Enable bool
    Set['WEmote_1']          = 30         -- Weight 
    Set['WEmoteType_1']      = "salute"   -- Animation Type

------ Do not change ----------------------------------------------------------
    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
--------------------------------------------------------------------------------
    self:SetVar("collideswitch", 1)
    self:SetVar("pauseswitch", 1)

end

function onNotifyObject(self, msg)

    if msg.name == "hit" then
        print "HIT!"
    end

end