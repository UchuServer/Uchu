require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')

CONSTANTS = {}
CONSTANTS["RebuildStateOpen"] = 0
CONSTANTS["RebuildStateCompleted"] = 2
CONSTANTS["EnemySmashableFaction"] = 1


function onStartup(self) 
    --self:SetVar("TickTime", 1)
end


--------------------------------------------------------------------------------
-- onRebuildNotifyState
-- 
-- Notes: Whenever the rebuild state changes Update
--------------------------------------------------------------------------------
function onRebuildNotifyState(self, msg)
    
	if (msg.iState == CONSTANTS["RebuildStateCompleted"]) then
	
	  
	     -- Set to Darkling hated smashable faction Using PLAYER faction for now.
	   -- self:SetFaction{ faction = CONSTANTS["EnemySmashableFaction"] }
	    GAMEOBJ:GetZoneControlID():NotifyObject{ name="built", ObjIDSender= self}
	 
    
	end
	
	if (msg.iState == CONSTANTS["RebuildStateOpen"]) then
			
	end
	
end
--[[
function onDie(self, msg)
    --GAMEOBJ:GetZoneControlID():NotifyObject{ name="qbdead", param1 = 200, ObjIDSender = msg.killerID}
end
--]]