require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')

CONSTANTS = {}
CONSTANTS["RebuildStateOpen"] = 0
CONSTANTS["RebuildStateCompleted"] = 2
CONSTANTS["EnemySmashableFaction"] = 1


function onStartup(self) 
    self:SetVar("TickTime", 1)
end


--------------------------------------------------------------------------------
-- onRebuildNotifyState
-- 
-- Notes: Whenever the rebuild state changes Update
--------------------------------------------------------------------------------
function onRebuildNotifyState(self, msg)
    
	if (msg.iState == CONSTANTS["RebuildStateCompleted"]) then
	
	  
	     -- Set to Darkling hated smashable faction Using PLAYER faction for now.
	    self:SetFaction{ faction = CONSTANTS["EnemySmashableFaction"] }
	    
	    self:CastSkill{skillID = 200}
	    self:SetVar("DieTime", 25)
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("DieTime")  , "DieTime", self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
    
	end
	
	if (msg.iState == CONSTANTS["RebuildStateOpen"]) then
			
	end
	
end



--Gets the player that built the generator
-- Commented out for the time being just in case we need it.
--[[
function onRebuildStart(self, msg)
    storeObjectByName (self, "playerID", msg.userID )
   
end
--]]

-------------------------
-- Check if the timer for self death is done.
-------------------------
onTimerDone = function(self, msg)
    if msg.name == "DieTime" then
         self:Die()
    end
    if msg.name == "TickTime" then
         self:CastSkill{skillID = 200}
          GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
    end
end