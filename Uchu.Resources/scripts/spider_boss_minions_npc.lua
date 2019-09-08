require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')
function onStartup(self) 
Set = {}

  
    -- use native code AI?
    Set['SuspendLuaAI']	   = true	           -- a state suspending scripted AI
    Set['SuspendLuaMovementAI']	   = true      -- a state suspending scripted movement AI
    

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
    Set['WanderDelayMin']    = 5            -- Min Wander Delay
    Set['WanderDelayMax']    = 5            -- Max Wander Delay
    Set['WanderSpeed']       = 0.5          -- Move speed 
    Set['wanderRadius']		 = 50			-- Wander Radius
    -- effect 1
    Set['WanderEmote']       = false        -- Enable bool
    Set['WEmote_1']          = 30           -- Weight 
    Set['WEmoteType_1']      = "salute"     -- Animation Type
	

------ Set your Custom ProximityRadius            -----------------------------

 --self:SetProximityRadius { radius = 10 , name = "CustomRadius" }
	
------ Do not change ----------------------------------------------------------
    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
--------------------------------------------------------------------------------

end



function onTemplateDie (self, msg ) 


	local ActivityObj = self:GetParentObj().objIDParent
	local pos =  self:GetPosition().pos
	local pos1 = ""..pos.x..","..pos.y..","..pos.z..""
	ActivityObj:NotifyObject{param1 = 2469 , paramObj = self, name = pos1 }
	ActivityObj:NotifyObject{name = "MinionDied" , paramObj = self }


end 
