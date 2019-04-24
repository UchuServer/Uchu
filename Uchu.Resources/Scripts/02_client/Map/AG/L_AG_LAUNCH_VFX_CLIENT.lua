--------------------------------------------------------------
-- client side ag luanch vfx
--
-- created mrb... 6/1/11
--------------------------------------------------------------

local missionID = 768

function onRenderComponentReady(self, msg)
	-- Obtain the local player object
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() or not player:GetPlayerReady().bIsReady then
		-- Subscribe to a zone control object notification alerting the script
		-- when the local player object is ready
		self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName = "PlayerReady"}
		
		return
	end
	
	-- Custom function to check the status of the plinth player flag
	CheckVis(self, player)
end

----------------------------------------------
-- The zone control object says when the player is loaded
----------------------------------------------
function notifyPlayerReady(self, zoneObj, msg)
    -- Cancel the notification request
    self:SendLuaNotificationCancel{requestTarget = zoneObj, messageName = "PlayerReady"}
    
    -- Get the player
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	
    -- Custom function to check the status of the plinth player flag
    CheckVis(self, player)
end

function CheckVis(self, player)
	-- see if the player has completed the mission
	if player:GetMissionState{missionID = missionID}.missionState < 8 then return end
	
	-- delete the objects
	GAMEOBJ:DeleteObject(self)
end 