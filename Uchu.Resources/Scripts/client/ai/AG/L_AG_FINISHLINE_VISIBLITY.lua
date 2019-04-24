-- This script is attached to the start/finish line objects in AG.
-- It handles showing or hiding the objects after determining if 
-- the player has previously built the finish line.

local finishLineFlag = 33

function onStartup(self)
	local playerID = GAMEOBJ:GetLocalCharID()
    local player = GAMEOBJ:GetObjectByID(playerID)
    
    -- make sure the player is ready
	if player:Exists() then		    
		-- check if the player flag has been set
		if player:GetFlag{iFlagID = finishLineFlag}.bFlag then 
			--print("Showing finish line")
			self:SetVisible{visible = true}
			self:SetCollisionGroupToOriginal()
		else
			--print("Hiding finish line")
			self:SetVisible{visible = false}
			self:SetCollisionGroup{colGroup = 16}
		end     
	else
		-- the player is not ready (i.e. not added to the world yet), 
		-- start a timer to check again in a sec
		--print("Player not ready, retrying in 1 sec")
		GAMEOBJ:GetTimer():AddTimerWithCancel(1.0, "RetryStartup", self)
		
		-- also, initially hide the finish line
		--print("Hiding finish line")
		self:SetVisible{visible = false}
	end
end

function onTimerDone(self, msg)
    if (msg.name == "RetryStartup") then
		-- try again...
		onStartup(self)
	end
end
