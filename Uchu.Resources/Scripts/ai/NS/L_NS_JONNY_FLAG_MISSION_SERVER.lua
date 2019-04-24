--------------------------------------------------------------
-- Script on Johnny Thunder, script adds missions for concert flyer from concert audience
-- 
-- updated Brandi... 3/29/10
--------------------------------------------------------------

function onMissionDialogueOK(self,msg)
	
	local player = msg.responder
	
	if msg.missionID == 773 and msg.iMissionState <= 2 then
	
		player:AddMission{ missionID = 774}
		player:AddMission{ missionID = 775}
		player:AddMission{ missionID = 776}
		player:AddMission{ missionID = 777}
		
	end
end