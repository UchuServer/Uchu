require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main_Bull')

function onStartup(self)

    self:SetVar("Stunned", 1 )
    -- if you really want to do this, do it in the handling of PhysicsComponentReady, not here
	--self:OverrideFriction{bEnableOverride = true, fFriction = 400}
	
    --self:AddObjectToGroup{ group = "hitboxes" }

    local placeposx = self:GetPosition{}.pos.x
    local placeposy = self:GetPosition{}.pos.y
    local placeposz = self:GetPosition{}.pos.z

    --RESMGR:LoadObject {objectTemplate = 6625, x = placeposx, y = placeposy, z = placeposz - 5, owner = self}

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

    Set['aggroRadius']      = 50     -- Aggro Radius
    Set['conductRadius']    = 50     -- Conduct Radius
    Set['tetherRadius']     = 100     -- Tether  Radius
    Set['tetherSpeed']      = 5      -- Tether Speed
    Set['wanderRadius']     = 15      -- Wander Radius
    -- Aggro
    Set['UseAggroFOV']      = false
    Set['aggroFOV']         = 360
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
    Set['AggroSpeed']      = 3             -- Multiplier of the NPC's base speed to approach while attacking

    -- Aggro Emote
    Set['AggroEmote']      = true          -- Plays Emote on Aggro 
    Set['AggroE_Type']     = "charge"            -- String Name of Emote
    Set['AggroE_Delay']    = 2             -- Animation Delay Time

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
    Set['WanderChance']      = 100        -- Main Weight
    Set['WanderDelayMin']    = 1          -- Min Wander Delay
    Set['WanderDelayMax']    = 1          -- Max Wander Delay
    Set['WanderSpeed']       = 0.65          -- Move speed 
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
    self:SetImmunity{ immunity = true }
end

function onCollisionPhantom(self, msg)
    local target = msg.objectID
	local faction = target:GetFaction()
    local BullVelx = self:GetLinearVelocity().linVelocity.x
    local BullVelz = self:GetLinearVelocity().linVelocity.z

    if faction and faction.faction == 1 then 
        if self:GetVar("CurrentState") == "aggro" and self:GetVar("Stunned") == 1 then
            self:FollowTarget { targetID = target,radius = 3, speed = 0, keepFollowing = false }
            self:SetVar("Stunned", 0 )
            self:SetImmunity{ immunity = false }
            GAMEOBJ:GetTimer():AddTimerWithCancel(  5.0 , "Bullimmune", self )
            self:Knockback{vector={ x = BullVelx, y = 15, z = BullVelz }}
            self:CastSkill{ skillID = 217 }
            self:PlayAnimation{animationID = "slip"}
        end
    end

end
