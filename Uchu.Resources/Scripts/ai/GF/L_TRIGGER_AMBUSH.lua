--Created: 6/7/09 bla, borrowed heavily from QA_MESSAGEBOX


-- OnEnter in HF Trigger system
function onCollisionPhantom(self, msg)
    -- Gets the target id that has collided
	if self:GetVar("triggered") ~= true then
		--print ("You entered")
		self:SetVar("triggered", true)
		--LEVEL:ActivateSpawner("Ambush")
		local ambushSpawner = LEVEL:GetSpawnerByName("Ambush")
		if ambushSpawner then
			ambushSpawner:SpawnerActivate()
		end
		GAMEOBJ:GetTimer():AddTimerWithCancel( 45, "TriggeredTimer", self )
		
	end
end

--[[ OnExit in HF Trigger system
function onOffCollisionPhantom(self, msg )
    -- Says we have finished colliding tries to resetBox()
       --LEVEL:DeactivateSpawner("Ambush")
	   print ("Exiting")
end
]]--

function onTimerDone(self, msg)    
    if msg.name == "TriggeredTimer" then
		--print ("timer done")
		self:SetVar("triggered", false)
		--LEVEL:ResetSpawner("Ambush")
		local ambushSpawner = LEVEL:GetSpawnerByName("Ambush")
		if ambushSpawner then
			ambushSpawner:SpawnerReset()
		end
	end
end