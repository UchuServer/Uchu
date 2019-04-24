--------------------------------------------------------------
-- Description:
--
-- Server script for Shooting Gallery NPC in GF area.
-- This NPC will react to a user interaction and prompt
-- the user to start the shooting gallery. If the user 
-- presses yes, the NPC will send him to the GF SG instance.
--
--------------------------------------------------------------

function onStartup(self)
	
end

--------------------------------------------------------------
-- Sent from a player when trying to use this object
--------------------------------------------------------------
--------------------------------------------------------------
-- Sent from a player when trying to use this object
--------------------------------------------------------------
function onUse(self, msg)
	
	local player = msg.user

    if player:CheckPrecondition{ PreconditionID = 32,iPreconditionType = 7 }.bPass then
    	self:NotifyClientObject{name = "Clicked", paramObj = player, rerouteID = msg.player}
    else
    	print("server click")
    	self:NotifyClientObject{name = "PreconditionFail", paramObj = player, rerouteID = msg.player}
    end
   	
end