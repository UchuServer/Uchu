--------------------------------------------------------------
-- Server Script on the jailkeep in Gnarled Forest
-- this script adds all the feed the ninjas missions and alerts the ninjas to show their icons

-- updated Brandi... 2/22/10
--------------------------------------------------------------

-- when the player accepts the jailkeep mission, this gives him all of the ninjas feed me missions
function onMissionDialogueOK(self,msg)
	print("mission state "..msg.iMissionState)
	local player = msg.responder
	
	if msg.missionID == 385 and msg.iMissionState == 1 then
	
		player:AddMission{ missionID = 386}
		player:AddMission{ missionID = 387}
		player:AddMission{ missionID = 388}
		player:AddMission{ missionID = 390}
			
		--get all the ninjas by group and tell them to show their icons
		local ninjas = self:GetObjectsInGroup{ group = 'Ninjas', ignoreSpawners = true }.objects
        
        for k,v in ipairs(ninjas) do 
           
            --v:FireEventClientSide{senderID = self, args = 'showIcon'}
			
        end
	
	-- turn the icons on the ninjas back off, so the player doesn't see the next ninja mission yet
	elseif msg.missionID == 385 and msg.iMissionState == 4 then
	
		local ninjas = self:GetObjectsInGroup{ group = 'Ninjas', ignoreSpawners = true }.objects
        
        for k,v in ipairs(ninjas) do 
           
            --v:FireEvent{senderID = self, args = 'hideIcon'}
			
        end
		
		-- if the player completes the jail keep mission after getting the free the ninjas mission, it re-adds the missions for the player
		if (player:GetFlag{iFlagID = 68}.bFlag == true) then 
		
			player:AddMission{ missionID = 701}
			player:AddMission{ missionID = 702}
			player:AddMission{ missionID = 703}
			player:AddMission{ missionID = 704}
			
		end
		
	end
	
end