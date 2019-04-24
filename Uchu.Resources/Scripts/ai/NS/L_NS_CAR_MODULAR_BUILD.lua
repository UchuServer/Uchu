---------------------------------------------------------------------------------------
--This script is on the car module build area in Race Place of Nimbus Station
-- it checks to see if the player has built a car. Since this is the first time a player can
-- build a car, it assumes that the car in the player's inventory is the car they just built
---------------------------------------------------------------------------------------


function onModularBuildExit(self, msg)
    -- the player completed a modular build
    if(msg.bCompleted == true) then
		-- the modular build is a car
		if msg.modularBuildID == 8092 then
			local player = msg.playerID
			-- if the player is on the mission, update the mission
			if player:GetMissionState{missionID = 623}.missionState == 2 then 
				player:UpdateMissionTask{taskType = "complete", value = 623, value2 = 1, target = self}
			end
		end
	end
end 

