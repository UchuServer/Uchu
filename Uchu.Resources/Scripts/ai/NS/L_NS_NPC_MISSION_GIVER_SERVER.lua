--------------------------------------------------------------
-- scripts on the faction rep to update a mission when the player choices a speciality kit for the first time
-- 
-- updated Brandi... 3/19/10
--------------------------------------------------------------
local Missions = {LOT_7095 = 812,LOT_7094 = 814,LOT_7097 = 813,LOT_7096 = 815}

function onMissionDialogueOK(self,msg)
	
	local player = msg.responder
	
	if  msg.iMissionState == 4 then
	
	-- check to see if the mission the player is completing is a feed me mission, if it is, hide their icons and skip the rest
		if msg.missionID == Missions["LOT_"..self:GetLOT().objtemplate] then
			player:UpdateMissionTask{taskType = "complete", value = 780, value2 = 1, target = self}
		end
	end
end