
-----------------------------------------------------------
-- Deactivate spawner areas if all mobs in area have been killed
-- Updated 3/24 Darren McKinsey
-----------------------------------------------------------
require('o_mis')
CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"

--the amount of mobs allowed before shutting off the area
local mobTarget = 0

-- the table of wall effects
local wallGroupNames = {"wallEffectA","wallEffectB"} --,"wallEffectC"}

-- the table of spawnernetworks
local spawnerNamesA = {"strombieA","mechA"} --,"wallEffect_03"}
local spawnerNamesB = {"strombieB","mechB"} --,"wallEffect_03"}
local spawnerGroups = {spawnerNamesA,spawnerNamesB}

local groupA = 1
local groupB = 1

--local missionState = player:GetMissionState{missionID = 785}.missionState

function onPlayerReady(self, msg)
--print("startup")
	
-- set spawners to be notified when something dies that they spawned
	for i,spawnerGroup in ipairs (spawnerGroups) do
		for i,spawnerName in ipairs (spawnerGroup) do
			local spawner = LEVEL:GetSpawnerByName(spawnerName)
			if spawner then
				self:SendLuaNotificationRequest{requestTarget = spawner, messageName = "NotifySpawnerOfDeath"}
			end			
		end
	end
	
-- turn on wall effects
	for i,groupName in ipairs (wallGroupNames) do
		local wallFX = self:GetObjectsInGroup{ group = groupName ,ignoreSpawners = true }.objects
		if wallFX then
			for i = 1, #wallFX do
				DoObjectAction(wallFX[i], "effect", "beam")
			end
		end
	end
end


-- When death notified the all spawn networks are checked for having no mobs on them, if the set of networks has zero mobs the corresponding area is opened up.
function notifyNotifySpawnerOfDeath(self, other, msg)
	--  add currently spawned mobs from each network in group

	print("mob death")
	
	for i,spawnerGroup in ipairs (spawnerGroups) do
		currentSpawned = 0

		for i,spawnerName in ipairs (spawnerGroup) do
			local spawner = LEVEL:GetSpawnerByName(spawnerName)
			mobs = spawner:SpawnerGetCurrentSpawned().iSpawned
			currentSpawned = currentSpawned + mobs
		end
			
		-- print(currentSpawned)
		
		for i,spawnerName in ipairs (spawnerGroup) do
			local spawner = LEVEL:GetSpawnerByName(spawnerName)
			if spawner then
				if currentSpawned == mobTarget then

					-------------------------------------
					-- Section for each Spawner Group  --
					-------------------------------------
					if spawnerGroup == spawnerNamesA and groupA == 1 then
						--print("turn off groupA")
						groupA = 0
						
						-- shut off spawner networks
						for i,spawnerName in ipairs (spawnerGroup) do
							local spawnerOFF = LEVEL:GetSpawnerByName(spawnerName)
							spawnerOFF:SpawnerDeactivate()
						end
					
						-- shut off physics
						local object = self:GetObjectsInGroup{group = "physics_01", ignoreSpawners = true}.objects[1]	
						if object then
							object:SetVar("Active", false)
						end
						
						
						-- shut off maelstrom effects
						local wallFX = self:GetObjectsInGroup{ group = "wallEffectA" ,ignoreSpawners = true }.objects
						if wallFX then
							for i = 1, #wallFX do
								DoObjectAction(wallFX[i], "stopeffects", "beam")
							end
						end	
					end
					-------------------------------------
					
					
					-------------------------------------
					-- Section for each Spawner Group  --
					-------------------------------------
					if spawnerGroup == spawnerNamesB and groupB == 1 then
						--print("turn off groupB")
						groupB = 0
						-- shut off spawner networks
						for i,spawnerName in ipairs (spawnerGroup) do
							local spawnerOFF = LEVEL:GetSpawnerByName(spawnerName)
							spawnerOFF:SpawnerDeactivate()
						end
						
						-- shut off physics							
						local object = self:GetObjectsInGroup{group = "physics_02", ignoreSpawners = true}.objects[1]
						if object then
							object:SetVar("Active", false)
						end

						-- shut off maelstrom effects
						local wallFX = self:GetObjectsInGroup{ group = "wallEffectB" ,ignoreSpawners = true }.objects
						if wallFX then
							for i = 1, #wallFX do
								DoObjectAction(wallFX[i], "stopeffects", "beam")
							end
						end
					end							
				end
			end
		end
	end
end
	

 
