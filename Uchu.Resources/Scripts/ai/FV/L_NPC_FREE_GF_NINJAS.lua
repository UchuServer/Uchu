--------------------------------------------------------------
-- Server Script on the ninja guy in Forbidden valley
-- this script adds all the free the ninjas missions 

-- updated Steve... 3/24/10
--------------------------------------------------------------

-- when the player accepts the free the ninjas mission, this gives him all of the ninjas free me missions
function onMissionDialogueOK(self,msg)

	local player = msg.responder
	
	if msg.missionID == 705 and msg.iMissionState == 1 then
	
		player:AddMission{ missionID = 701}
		player:AddMission{ missionID = 702}
		player:AddMission{ missionID = 703}
		player:AddMission{ missionID = 704}
		
		player:SetFlag{iFlagID = 68, bFlag=true}
		
	elseif msg.missionID == 786 then
    
        player:SetFlag{iFlagID = 81, bFlag=true}
    
    end
	
end