---------------------------------------------------------------------------------------
--This script is on the rocket module build area in Brick annex of Nimbus Station
-- It checks the rocket the player build in the modular build area for certain rocket part
-- if the player builds a rocket and it contains the defined modules, update the mission
---------------------------------------------------------------------------------------

local iTemplateID = {9516,9517,9518}  -- the LOT of the module within the iAssemblyTemplate you are searching for.
local missionNum = 809 -- mission on mardolf to build a rocket with a certain rocket part

-- The player exits the modular build
function onModularBuildExit(self, msg)
    -- has the player completed a rocket?
    if(msg.bCompleted == true) then
		local player = msg.playerID
		-- is the player on the mission to build a rocket
		if player:GetMissionState{missionID = missionNum}.missionState == 2 then 
			if msg.vLOTsUsed then
				for i,v in ipairs(msg.vLOTsUsed) do
					--see if any of the LOTs used are for our mission
					for j,t in ipairs(iTemplateID) do
						if v == t then
							player:UpdateMissionTask{taskType = "complete", value = missionNum, value2 = 1, target = self}
							return nil
						end
					end
				end
			end
			for k,v in ipairs(iTemplateID) do
				-- send a check to the character database to see if the rocket contains a certain
				player:CheckPlayerAssemblyForUniqueModuleByLOT{iObjTemplate = v, 
						iAssemblyTemplate = msg.modularBuildID, i64modelSubkey = msg.i64modelSubkey:GetID(), 
						callbackTarget = self, playerID = player}
			end
		end
	end

end 

-- check return from the character database check
function onModuleAssemblyDBDataToLua(self, msg)
	--
	if msg.bDataReady == false then
		msg.playerID:DisplayTooltip{ bShow = true, strText = Localize("MODULAR_ROCKET_DB_FAIL")} --"Internal Database Error. Please build your rocket again"}
    -- does the rocket contain one of the define modules?
	elseif msg.bModuleFound == true then 
		local player = msg.playerID
		-- if the player is on the mission, update the mission
		if player:GetMissionState{missionID = missionNum}.missionState == 2 then 
			player:UpdateMissionTask{taskType = "complete", value = missionNum, value2 = 1, target = self}
		end
	end

end