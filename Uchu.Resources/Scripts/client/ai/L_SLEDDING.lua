CONSTANTS = {}
CONSTANTS["cooldown"] = 2.0

function onStartup(self)
	self:SetVar("TimerActive", false)

end

function onCollisionPhantom(self, msg)
	local target = msg.objectID
    	
	local faction = target:GetFaction()
	
	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 1 then
		if (self:GetVar("TimerActive") == true) then
		else

			local params = target:GetSleddingState().bSleddingState

			if (tostring(params) ~= "true") then 
				target:SetSleddingState{bSleddingState = true}	
			else
				target:SetSleddingState{bSleddingState = false}
			end
			GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["cooldown"], "CoolDown",self ) 	
			self:SetVar("TimerActive", true)	
		end
    	end
	
	return msg
end
        
--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)

    -- parse the name to get out the wave number
    -- use the wave number to select the spawns
    -- of format "SpawnWaveXXX" where XXX is the spawn number
    
    if (msg.name == "CoolDown") then 
	self:SetVar("TimerActive", false)
    end

end
