

function onNotifyObject(self, msg)
	if ( msg.name == "CorrectTimer" ) then
	
		GAMEOBJ:GetTimer():CancelTimer("RightTikiTimer", self)
		GAMEOBJ:GetTimer():AddTimerWithCancel(30, "RightTikiTimer", self )
		
	elseif ( msg.name == "RampTimer" ) then 
	
		GAMEOBJ:GetTimer():CancelTimer("RightTikiTimer", self)
		GAMEOBJ:GetTimer():AddTimerWithCancel(45, "TikiPuzzle", self )
	
	end
end

function resetEverything(self)
	local CHOICEBUILDS = self:GetObjectsInGroup{ group = 'TikiHeads', ignoreSpawners = true}.objects
	if CHOICEBUILDS ~= nil then
		for k,v in ipairs(CHOICEBUILDS) do

		v:Die()
		--print(v)
		end
	end
	--LEVEL:DestroySpawnerObjects("Ramp")	
	--LEVEL:ResetSpawner("Ramp")
	local rampSpawner = LEVEL:GetSpawnerByName("Ramp")
	if rampSpawner then
		rampSpawner:SpawnerDestroyObjects()
		rampSpawner:SpawnerReset()
	end
	local group = self:GetObjectsInGroup{ group = 'CBGroup', ignoreSpawners = true }.objects	
	for i = 1, #group do   
	-- test each object in table group to see if GetVar('TikiSet') is true, if so add 1 to tikiTest
		if group[i]:GetVar('TikiSet') then		
			group[i]:SetVar('TikiSet', false)
		end	
	 end	
end

function onTimerDone(self, msg)    
    if msg.name == "TikiPuzzle" then
		GAMEOBJ:GetTimer():CancelTimer("RightTikiTimer", self)
		print ("puzzle timer done")
		resetEverything(self)
		
	elseif msg.name == "RightTikiTimer" then
		print ("right timer done")
		resetEverything(self)
	
	end
end