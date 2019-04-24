--------------------------------------------------------------
-- Helper Functions for Shooting Gallery
--------------------------------------------------------------

--------------------------------------------------------------
-- Add a new wave
--------------------------------------------------------------
function AddWave(waves, time, text)
	local num = #waves + 1
	waves[num] = { timeLimit = time, waveStr = text }
end


--------------------------------------------------------------
-- Add a new path
--------------------------------------------------------------
function AddPath(paths, text)
	local num = #paths + 1
	paths[num] = text
end


--------------------------------------------------------------
-- Add a new spawn
--------------------------------------------------------------
function AddSpawn(spawn, paths, id, initSpawnTimeMin, initSpawnTimeMax, bRespawn, mintime, maxtime,
                  speed, score, bChangeSpeed, chanceChangeSpeed, minSpeed, maxSpeed,
                  bMovingPlatform, despawnTime, timeScore)
	local num = #spawn + 1
	spawn[num] = { id = id, initSpawnTimeMin = initSpawnTimeMin, initSpawnTimeMax = initSpawnTimeMax, 
	         bRespawn = bRespawn, minTime = mintime, maxTime = maxtime, 
	         speed = speed, score = score, bChangeSpeed = bChangeSpeed,
	         speedChangeChance = chanceChangeSpeed, minSpeed = minSpeed, 
			 maxSpeed = maxSpeed, bMovingPlatform = bMovingPlatform,
			 despawnTime = despawnTime, timeScore = timeScore,
			 path = paths }
end


--------------------------------------------------------------
-- Add spawns for a wave
--------------------------------------------------------------
function AddSpawnsForWave(spawns, spawn)
	local num = #spawns + 1
	spawns[num] = spawn
end


--------------------------------------------------------------
-- store an object by name
--------------------------------------------------------------
function storeObjectByName(varName, object)

    idString = object:GetID()
    LOCALS[varName] = idString
   
end


--------------------------------------------------------------
-- get an object by name
--------------------------------------------------------------
function getObjectByName(varName)

    targetID = LOCALS[varName]
    if (targetID) then
		return GAMEOBJ:GetObjectByID(targetID)
	else
		return nil
	end
	
end


--------------------------------------------------------------
-- check to see if a string starts with a substring
--------------------------------------------------------------
function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end


--------------------------------------------------------------
-- Increment a saved variable and return its new value
--------------------------------------------------------------
function IncrementVarAndReturn(varName)
	local value = LOCALS[varName]
	if (value) then
		value = value + 1
	end
	LOCALS[varName] = value
	return value
end
