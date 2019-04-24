--------------------------------------------------------------
--  Server script on smashable pipes for the blue brick puzzle
-- this script only works if the spawner networks are set up correctly, in numerical order 

-- created Brandi... 8/25/10
--------------------------------------------------------------

--------------------------------------------------------------
-- timer for the quickbuilds to reset if the player doesnt build them
--------------------------------------------------------------
local resetBricks = 30

--------------------------------------------------------------
-- when the object is loaded
--------------------------------------------------------------
function onStartup(self,msg)
	-- check to see if this object is the smashable only pipes
		-- get the name of the spawner network
	local mygroup = tostring(self:GetVar("spawner_name"))
	-- get the last value in the string and convert it to a number
	local pipeNum = tonumber(string.sub(mygroup,11,11))
	
	if pipeNum ~= 1 then
		-- start a timer for the quickbuilds to reset if the player doesnt build them
		GAMEOBJ:GetTimer():AddTimerWithCancel(resetBricks, "reset", self )
	end
end

--------------------------------------------------------------
-- when the object dies
--------------------------------------------------------------
function onDie(self,msg)
	-- get the name of the spawner network
	local mygroup = tostring(self:GetVar("spawner_name"))
	-- get the last value in the string and convert it to a number
	local pipeNum = tonumber(string.sub(mygroup,11,11))
	-- get the first part of the spawner network, without the number
	local pipeGroup = string.sub(mygroup,1,10)
	-- add 1 to the value of the spawner network
	local nextPipeNum = pipeNum + 1
	
	-- deactivate and reset current spawner network
	local SamePipeSpawner = LEVEL:GetSpawnerByName(mygroup)
	if SamePipeSpawner then
		SamePipeSpawner:SpawnerReset()
		SamePipeSpawner:SpawnerDeactivate()
	end
	
	-- see if the object was killed by the player, if so spawn the next object
	if msg.killerID:Exists() and msg.killerID:IsCharacter().isChar then
		-- create the string for the next spawner network
		local nextPipe = pipeGroup..nextPipeNum
		local NextPipeSpawner = LEVEL:GetSpawnerByName(nextPipe)
		if NextPipeSpawner then
			NextPipeSpawner:SpawnerActivate()
		end
	--if object died on its own, then reset the puzzle back to the beginning
	else
		-- create a string for the first spawner network
		local firstPipe = pipeGroup.."1"
		-- activate the first spawner network
		local FirstPipeSpawner = LEVEL:GetSpawnerByName(firstPipe)
		if FirstPipeSpawner then
			FirstPipeSpawner:SpawnerActivate()
		end
	end
end

--------------------------------------------------------------
-- when a timer is done
--------------------------------------------------------------
function onTimerDone(self, msg)
	-- timer on quickbuild when it first starts up
	if (msg.name == "reset") then
		-- if the quickbuild hasnt been built yet, kill it
		if self:GetRebuildState{}.iState == 0 then
			self:RequestDie{killerID = self, killType = "SILENT"}
		end
	end
end