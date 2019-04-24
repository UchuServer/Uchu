--------------------------------------------------------------

-- L_NT_XRAY_CLIENT.lua

-- Script attaching cinematic and functionality
-- created abeechler ... 2/15/11

--------------------------------------------------------------

function onCollisionPhantom(self, msg)
	local player = msg.senderID
	
	-- Check HF object config data for a cinematic path to play on xray
	local xRayCinematic = self:GetVar("Cinematic") or false
	if(xRayCinematic) then
		player:PlayCinematic{pathName = xRayCinematic, leadIn = 0.3}
	end
end

function onOffCollisionPhantom(self, msg)
	local player = msg.senderID
	self:SetVar("userID", player:GetID())
	
	-- Check HF object config data for a cinematic path
	-- If one exists, time out conclusion transition
	local xRayCinematic = self:GetVar("Cinematic") or false
	if(xRayCinematic) then
		GAMEOBJ:GetTimer():AddTimerWithCancel(1, "EndCinematic", self)
	end
end

function onTimerDone(self, msg)
	local player = GAMEOBJ:GetObjectByID(self:GetVar("userID"))
	if (not player:Exists()) then
		-- Catch the case where a timed interaction is initiated but 
		-- the player has disappeared
		return
	end
	
	if (msg.name == "EndCinematic") then
		-- Check HF object config data for a cinematic path to cancel
		local xRayCinematic = self:GetVar("Cinematic") or false
		if(xRayCinematic) then
			player:EndCinematic{pathName = xRayCinematic, leadOut = 0.5}
		end
	end
end
