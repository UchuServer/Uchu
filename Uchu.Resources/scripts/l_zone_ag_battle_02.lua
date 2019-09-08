
-----------------------------------------------------------
-- Manage local mission, reset mission on death || Manage spawners, and reset them on death
-- Updated 4/5 Darren McKinsey
-----------------------------------------------------------
require('o_mis')
CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"

local spawnerNames = {"spider_01","spider_02","spider_03","spider_04","smallCrates_01","smallCrates_02","mediumCrate_01"}
local missionNum = 808
local missionGiver = "missionGiver"


-- if the player loads in and the mission has not been completed then reset the mission
function onPlayerLoaded(self, msg)
	player = msg.playerID
	if player then
		self:SetVar("player", msg.playerID:GetID())
		if player:GetMissionState{missionID = missionNum}.missionState <= 4 then
			player:ResetMissions{missionID = missionNum}
		end
	end
	-- request notification when player accepts mission
	local object = self:GetObjectsInGroup{group = missionGiver, ignoreSpawners = true}.objects[1]
	if object then
		self:SendLuaNotificationRequest{requestTarget = object, messageName = "MissionDialogueOK" }
	end
end

-- reset spawners if the player kills a couple of mobs then accepts mission (otherwise they will not be able to complete the mission
function notifyMissionDialogueOK(self,other,msg)
	if (msg.iMissionState <= 2) then
        for i,spawnerName in ipairs (spawnerNames) do
			local spawner = LEVEL:GetSpawnerByName(spawnerName)
			if spawner then
				spawner:SpawnerActivate{}
				spawner:SpawnerReset{}
			end
		end 
    end
end

-- remove current mobs and reset spawners if the player dies
function onPlayerDied(self, msg)
	for i,spawnerName in ipairs (spawnerNames) do
		local spawner = LEVEL:GetSpawnerByName(spawnerName)
		if spawner then
			spawner:SpawnerDestroyObjects{bDieSilent = true}
			spawner:SpawnerActivate{}
			spawner:SpawnerReset{}
		end			
	end
	
	-- reset the players mission if the player has the mission and dies
	if msg.playerID:GetID() == self:GetVar("player") then
		local hasMission = player:GetHasMission{iMissionID = missionNum}.bHasMission
		if hasMission == true then
			if player:GetMissionState{missionID = missionNum}.missionState <= 4 then
				player:ResetMissions{missionID = missionNum}
				player:AddMission{missionID = missionNum}
			end
		end
    end
end

-- if player exits map without completing mission the mission is reset (removed)
function onPlayerExit(self, msg)
	local player = msg.playerID
	if player then
		if player:GetMissionState{missionID = missionNum}.missionState <= 4 then
			player:ResetMissions{missionID = missionNum}
		end
	end
end


