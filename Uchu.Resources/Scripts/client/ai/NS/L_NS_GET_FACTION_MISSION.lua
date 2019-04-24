-----------------------------------------------------------
-- This script is on the faction rep
-- after selecting a faction, the player does a celeb and when it's complete is 
-- offered the correct mission to go talk to their new faction rep
-- 
-- updated mrb... 3/9/11 - changed to event based logic
----------------------------------------------------------------

local misID = 474 -- join a faction mission

function onMissionDialogueOK(self,msg)	
	if not msg.responder:Exists() then return end
	
	if msg.missionID == misID and msg.iMissionState == 4 then
		-- set the lua notification to catch that the player has completed the celebration
		self:SendLuaNotificationRequest{requestTarget = msg.responder, messageName = "CelebrationCompleted"}
    end    
end

--------------------------------------------------------------
-- when the celebration is completed, put the env settings back
--------------------------------------------------------------
function notifyCelebrationCompleted(self, other, msg)
	-- make sure the player is still there
	if not other:Exists() then return end
	
	-- make the player the player interact to get the next mission
	other:ForcePlayerToInteract{ objToInteractWith = self }		
	-- clear the lua notification
	self:SendLuaNotificationCancel{requestTarget= other, messageName="CelebrationCompleted"}
end 