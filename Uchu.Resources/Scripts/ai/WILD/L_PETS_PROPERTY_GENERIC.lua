require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')

function onStartup(self) 
Set = {}

--New AI Override
------------------------------------
Set['SuspendLuaAI']          = true      -- a state suspending scripted AI
Set['SuspendLuaMovementAI']	   = true      -- a state suspending scripted movement AI
----------------------------------------
--[[
///////////////////////////////////////////////////////////////////////////
         ____       _      ____    ___   _   _   ____  
        |  _ \     / \    |  _ \  |_ _| | | | | / ___| 
        | |_) |   / _ \   | | | |  | |  | | | | \___ \ 
        |  _ <   / ___ \  | |_| |  | |  | |_| |  ___) |
        |_| \_\ /_/   \_\ |____/  |___|  \___/  |____/         
--]]

    Set['wanderRadius']     = 16      -- Wander Radius

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
    Set['WanderDelayMin']    = 2            -- Min Wander Delay
    Set['WanderDelayMax']    = 5            -- Max Wander Delay
    Set['WanderSpeed']       = 1          -- Move speed 
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
    self:SetVar("petclick", 1)
end

function onUse(self, msg)

    if self:GetVar("petclick") == 1 then
        self:SetVar("petclick", 0)
        self:PlayAnimation{ animationID = "scared" }
        self:SetStunned{StateChangeType = "PUSH", bCantMove = true, bIgnoreImmunity = true, bCantTurn = true, bCantEquip = true}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5.0, "UnStunPet", self )
    end

end

function onTimerDone(self, msg)

    if ( msg.name == "UnStunPet" ) then
        self:SetStunned{StateChangeType = "POP", bCantMove = true, bIgnoreImmunity = true, bCantTurn = true, bCantEquip = true}
        self:SetVar("petclick", 1)
    end

end