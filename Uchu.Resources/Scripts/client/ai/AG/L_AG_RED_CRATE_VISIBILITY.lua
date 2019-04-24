--------------------------------------------------------------
-- Script on the red crate in AG to make it invisible if the player isnt on the mission

-- 
-- created by brandi 3/9/10
--------------------------------------------------------------

function onRenderComponentReady(self,msg)
	
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
		-- make sure the player is ready
		if player:Exists() then
			
			local mission = player:GetMissionState{missionID = 746}.missionState
			
			-- if the player is not currently on the mission from Captain Jack to find his treasure,
			-- then make the crate invisible
			if mission ~= 2  then
				
				self:SetVisible{visible = false, fadeTime = 0}
				return
			end
			
		else
			
			-- if the player isnt fully loaded to check the player flag status, create 'heartbeat timer'
			GAMEOBJ:GetTimer():AddTimerWithCancel(1.0, "CheckPlayer", self)
		
		end
	
end


function onTimerDone(self,msg)

	if (msg.name == "CheckPlayer") then
	
	    onRenderComponentReady(self,msg)
		
	end
	
	
end
