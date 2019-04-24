--------------------------------------------------------------

-- L_NPC_EPSILON_CLIENT.lua

-- Client side script for Epsilon Starcracker, AG NPC
-- created abeechler - 6/10/11
--------------------------------------------------------------

local nexusMisID = 1851	        -- Finish this mission to join Nexus Force, initiating a celebration

----------------------------------------------
-- Catch and parse dialogue acceptance messages
----------------------------------------------
function onMissionDialogueOK(self,msg)
    -- Confirm player existence
    local player = msg.responder	
	
	local missionID = msg.missionID
	
	if((missionID == nexusMisID) and (msg.bIsComplete)) then
	    -- We are turning in the mission required to join Nexus Force,
		-- request lua notification to catch when the player has completed the celebration
		self:SendLuaNotificationRequest{requestTarget = player, messageName = "CelebrationCompleted"}
		
    end 
   
end

--------------------------------------------------------------
-- Receive notification on celebration completion
--------------------------------------------------------------
function notifyCelebrationCompleted(self, player, msg)
	-- Compel further player interaction with Epsilon
	player:ForcePlayerToInteract{objToInteractWith = self}		
	-- Clear the lua notification
	self:SendLuaNotificationCancel{requestTarget= player, messageName="CelebrationCompleted"}
	
end 
