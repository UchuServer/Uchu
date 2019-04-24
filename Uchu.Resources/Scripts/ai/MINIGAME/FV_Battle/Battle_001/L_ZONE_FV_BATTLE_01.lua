
require('o_mis')

local missionGiver = "missiongiver" 

-- the table of spawnernetworks
local spawnerNames1 = {"SpawnPath001","1"} 
local spawnerNames2 = {"SpawnPath002","2"} 
local spawnerNames3 = {"SpawnPath003","3"} 
local spawnerNames4 = {"SpawnPath004","4"} 

local spawnerGroups = {spawnerNames1,spawnerNames2,spawnerNames3,spawnerNames4}

local mobsKilled = 0

----------------------------------------------------------------------------
--  On Player Load Check for Battle Mission if True then Reset Mission
----------------------------------------------------------------------------
function onPlayerLoaded(self, msg)

	player = msg.playerID
	if player then
	
		storeObjectByName(self, "PLAYER", player)
		self:SetVar("player", msg.playerID:GetID() )
		if player:GetMissionState{missionID = 805}.missionState <= 4 then
			player:ResetMissions{missionID = 805}
		end
		-- if player is dead Resurrect
		if player:IsDead{}.bDead then
			player:Resurrect()
		end
		-- request notification when player accepts mission
		local object = self:GetObjectsInGroup{group = missionGiver, ignoreSpawners = true}.objects[1]
		if object then
			self:SendLuaNotificationRequest{requestTarget = object, messageName = "MissionDialogueOK" }
		end
		
	end
end
----------------------------------------------------------------------------
--  On Player Die Reset Battle Mission + Reset Stage
----------------------------------------------------------------------------
function onPlayerDied(self, msg)

	if msg.playerID:GetName().name == self:GetVar("player") then
	
		local hasMission = player:GetHasMission{iMissionID = 805}.bHasMission
		ResetBattle(self)
		if hasMission then
			
			if player:GetMissionState{missionID = 805}.missionState <= 10 then
				player:ResetMissions{missionID = 805}
			end
		end
    end
end


----------------------------------------------------------------------------
--  On Player Exit Reset Battle Mission
----------------------------------------------------------------------------
function onPlayerExit(self, msg)

	local player = msg.playerID
	if player then

		
			player:CancelMission{missionID = 805}
		
	end
end

----------------------------------------------------------------------------
--  Set NotifySpawnerOfDeath msg
----------------------------------------------------------------------------
function onPlayerReady(self, msg)

	
	-- set spawners to be notified when something dies that they spawned
	for i,spawnerGroup in ipairs (spawnerGroups) do
		for i,spawnerName in ipairs (spawnerGroup) do
			local spawner = LEVEL:GetSpawnerByName(spawnerName)
			if spawner then
				self:SendLuaNotificationRequest{requestTarget = spawner, messageName = "NotifySpawnerOfDeath"}
			end			
		end
	end
end
----------------------------------------------------------------------------
--  ON NotifySpawnerOfDeath msg
----------------------------------------------------------------------------
function notifyNotifySpawnerOfDeath(self, other, msg)

	mobsKilled = mobsKilled + 1
	
	if mobsKilled >= 40 then
        for i,spawnerGroup in ipairs (spawnerGroups) do
            for i,spawnerName in ipairs (spawnerGroup) do
                    local spawner = LEVEL:GetSpawnerByName(spawnerName)
                    if spawner then
                        spawner:SpawnerDestroyObjects{bDieSilent = true}
                        spawner:SpawnerDeactivate()
                    end			
            end
        end	
	end
	
end

----------------------------------------------------------------------------
--  ON Dialogue OK Button Press
----------------------------------------------------------------------------
function notifyMissionDialogueOK(self,msg)

    local player = getObjectByName(self, "PLAYER")
    local State = player:GetMissionState{missionID = 805}.missionState
	
    if State == 1 or State == 9 then
        -- Deactivate Spawners
		for i,spawnerGroup in ipairs (spawnerGroups) do
			for i,spawnerName in ipairs (spawnerGroup) do
					local spawner = LEVEL:GetSpawnerByName(spawnerName)
					if spawner then
						spawner:SpawnerDestroyObjects{bDieSilent = true}
						spawner:SpawnerDeactivate()
					end			
			end
		end	
		
    elseif State == 2 or State == 10 then
    
    	-- Activate Spawners
		for i,spawnerGroup in ipairs (spawnerGroups) do
			for i,spawnerName in ipairs (spawnerGroup) do
				local spawner = LEVEL:GetSpawnerByName(spawnerName)
				if spawner then
					spawner:SpawnerActivate()
					spawner:SpawnerReset()
				end			
			end
		end    
	    mobsKilled = 0		
    
    end
    
    
end
----------------------------------------------------------------------------
--  Reset Network Spawners
----------------------------------------------------------------------------
function ResetBattle(self)

    for i,spawnerGroup in ipairs (spawnerGroups) do
        for i,spawnerName in ipairs (spawnerGroup) do
                local spawner = LEVEL:GetSpawnerByName(spawnerName)
                if spawner then
                    spawner:SpawnerActivate()
                    spawner:SpawnerReset()
                end			
        end
	end
	
end