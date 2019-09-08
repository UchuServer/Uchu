require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')


function onStartup(self) 
--------------------------------------        
--add the spider to a group        
--------------------------------------
    self:AddObjectToGroup{group = "SpiderDen"}
    self:SetVar("TickTime", 5)
    self:SetVar("killTurret1", false)
    self:SetVar("killTurret2", false)
    self:SetVar("killMovingPlatform", false)
    self:SetVar("amStunned", false)
    self:SetVar("StunTime", 15)
    self:SetVar("amHit", false)
    self:SetVar("HitTime", 1.8)
    --print("spider boss 2 starting up!")

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

    Set['aggroRadius']      = 135    -- Aggro Radius
    Set['conductRadius']    = 135    -- Conduct Radius
    Set['tetherRadius']     = 135    -- Tether  Radius
    Set['tetherSpeed']      = 0      -- Tether Speed
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
    Set['AggroDist']      = 135         -- Distance away from target to stop before attacking
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


end

function onNotifyObject(self, msg)
    if (msg.name == "killQBTurret1") or (msg.name == "killQBTurret2") then
        self:SetFaction{faction = 107}
        --print("faction set to 107 for spider")
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
        if (msg.name == "killQBTurret1") then
            self:SetVar("killTurret1", true)
        else
            self:SetVar("killTurret2", true)
        end
    elseif (msg.name == "dontShootQBTurret1") and (self:GetVar("killMovingPlatform" == true)) or (msg.name == "dontShootQBTurret2") and (self:GetVar("killMovingPlatform" == true)) then
        if (msg.name == "dontShootQBTurret1") then
            self:SetVar("killTurret1", false)
        else
            self:SetVar("killTurret2", false)
        end
        if (self:GetVar("killTurret1") == false) and (self:GetVar("killTurret2") == false) then
            --self:SetFaction{faction = 109}
            --print("***spider faction set to 109***")
            GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
        end
    elseif (msg.name == "dontShootQBTurret1") or (msg.name == "dontShootQBTurret2")then
        if (msg.name == "dontShootQBTurret1") then
            self:SetVar("killTurret1", false)
        else
            self:SetVar("killTurret2", false)
        end
        if(self:GetVar("killTurret1") == false) and (self:GetVar("killTurret2") == false) then
            self:SetFaction{faction = 38}
            print("spider faction set to 38")
            GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
        end
    elseif (msg.name == "killPlatform") then
        self:SetVar("killMovingPlatform", true)
        --self:SetFaction{faction = 109}
        --print("*_*spider faction set to 109*_*")
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
    elseif (msg.name == "platformDead") and (self:GetVar("killTurret1") == false) and (self:GetVar("killTurret2") == false) then
        self:SetVar("killMovingPlatform", false)
        self:SetFaction{faction = 38}
        --print("spider faction set to 38!")
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
    elseif (msg.name == "superTurretFired") then
        print("super turret fired!")
        self:SetFaction{faction = 110}
        self:SetVar("amStunned", true)
        self:SetVar("amHit", true)
        local friends = self:GetObjectsInGroup{group = "SpiderDen", ignoreSpawners = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 6626) then
                object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
            end
        end
        self:PlayAnimation{animationID = "spider-laser"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("HitTime")  , "HitTime", self )
    end
end

function onTimerDone(self, msg)
    print("spider boss timer finished")
    if (self:GetVar("amHit") == true) and (self:GetVar("amStunned") == true) then
        print("spider getting hit!")
        self:SetVar("amHit", false)
        local friends = self:GetObjectsInGroup{group = "SpiderDen", ignoreSpawners = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 6626) then
                object:StopFXEffect{name = "moviespotlight"}
            end
        end
        self:PlayAnimation{animationID = "spider-stun"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("StunTime")  , "StunTime", self )
    elseif (self:GetVar("amHit") == false) and (self:GetVar("amStunned") == true) then
        print("spider no longer stunned")
        self:SetVar("amStunned", false)
        self:SetFaction{faction = 38}
        self:PlayAnimation{animationID = "idle"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
    elseif (self:GetVar("amStunned") == false) and (self:GetVar("amHit") == false) then
        print("pPulse, spider not stunned!")
        ProximityPuls(self)
    end
end

function onOnHit(self, msg)
    print("spider got hit!")
end
