--------------------------------------------------------------
-- Client side script for Sky Lane, AG NPC

-- created mrb - 9/20/11
--------------------------------------------------------------

local npcMissionID = 320 -- mission to goto NS

----------------------------------------------
-- Catch and parse dialogue acceptance messages
----------------------------------------------
function onMissionDialogueOK(self,msg)
    -- Confirm player existence
    local player = msg.responder	
	
	local missionID = msg.missionID
	
	if missionID == npcMissionID and msg.bIsComplete and player:GetIsUsingFreeTrial().bFreeTrial then
		UI:SendMessage("popGameState", {{"state", "mission"}})
		UI:SendMessage("ToggleFreeTrialCallToAction", {{"visible", true}, {"strIdentifier", "CallToAction"}, {"callbackClient", self}})
	end
end 