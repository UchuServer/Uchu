require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')

function onStartup(self)

    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "SnakeStartup",self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.3, "SnakeFollow",self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "SnakeGlow",self )

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
    Set['conductRadius']    = 15     -- Conduct Radius
    Set['tetherRadius']     = 50     -- Tether  Radius
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

    Set['Aggression']     = "Passive"  -- [Aggressive]--[Neutral]--[Passive]
									      -- [PassiveAggres]-
    Set['AggroNPC']        = true
    Set['AggroDist']      = 7          -- Distance away from target to stop before attacking
    Set['AggroSpeed']     = 3.5         -- Multiplier of the NPC's base speed to approach while attacking
    

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
    Set['WanderChance']      = 0          -- Main Weight
    Set['WanderDelayMin']    = 9999         -- Min Wander Delay
    Set['WanderDelayMax']    = 9999         -- Max Wander Delay
    Set['WanderSpeed']       = 0.5          -- Move speed 
    Set['WanderEmote']       = false        -- Enable bool
    Set['WEmote_1']          = 30           -- Weight 
    Set['WEmoteType_1']      = "salute"     -- Animation Type
	

------ Set your Custom ProximityRadius            -----------------------------

 --self:SetProximityRadius { radius = 40 , name = "CustomRadius" }
	
------ Do not change ----------------------------------------------------------
    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
--------------------------------------------------------------------------------
end

function onDie(self, msg)

    local snakes = self:GetObjectsInGroup{ group = "snakesonaplane" }.objects

    for i = 1, table.maxn (snakes) do 
        snakes[i]:NotifyObject{name = "next1", ObjIDSender = self}
    end

end

function onTimerDone(self, msg)

	if msg.name == "SnakeStartup" then
        self:AddObjectToGroup{ group = "snakesonaplane" }
        self:OverrideFriction{bEnableOverride = true, fFriction = 10}
        self:SetFaction{faction = 4}

    elseif msg.name == "SnakeFollow" then

        local friends = self:GetObjectsInGroup{ group = "snakesonaplane" }.objects

        for i = 1, table.maxn (friends) do 
            if friends[i]:GetLOT().objtemplate == 6567 then
                self:FollowTarget { targetID = friends[i], radius = 4.5, speed = 1, keepFollowing = true }
            end
        end

    elseif msg.name == "SnakeGlow" then
        self:PlayFXEffect{effectType = "red"}
    end

end 