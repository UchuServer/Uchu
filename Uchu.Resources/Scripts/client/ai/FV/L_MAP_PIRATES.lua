--------------------------------------------------------------
-- Client Script pirates in FV, they dont render if the player has completed the mission for them

-- created Brandi... 8/4/10 
-- updated by brandi... 4/14/11 - removed heartbeat timer
--------------------------------------------------------------

local freeMis = {LOT_8657 = 740,LOT_8658 = 741,LOT_8659 = 742}

function onScopeChanged(self,msg)
	-- if the player entered ghosting range
    if msg.bEnteredScope then  
	-- get the player
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		if not player:Exists() then 
			-- tell the zone control object to tell the script when the local player is loaded
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
			return
		end
		-- custom function
		CheckMissions(self,player)
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckMissions(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end


function CheckMissions(self,player)
	-- check to see if the player has taken the mission from Olivia and not healed Rutger net
	if player:GetMissionState{missionID = freeMis["LOT_"..self:GetLOT().objtemplate]}.missionState == 8 then
		self:SetVisible{visible = false, fadeTime = 0}
	end
	
end

