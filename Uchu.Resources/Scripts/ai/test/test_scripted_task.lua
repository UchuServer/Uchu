--When a player clicks on me, send an update mission task message to the mission system
--This is attached to object 6944
--This triggers mission 446, the "eaten by sharks" mission

function onUse(self, msg) 

	--msg.user:UpdateMissionTask{taskType = "complete", value = [MISSION_ID], value2 = [how many], target = [lot from DB] }
	--note that "complete" in this context just means "script task", it doesn't actually mean to complete the entire mission
	
	msg.user:UpdateMissionTask{taskType = "complete", value = 445, value2 = 1, target = self}
	msg.user:UpdateMissionTask{taskType = "complete", value = 446, value2 = 1, target = self}
	msg.user:UpdateMissionTask{taskType = "complete", value = 447, value2 = 1, target = self}
    
end