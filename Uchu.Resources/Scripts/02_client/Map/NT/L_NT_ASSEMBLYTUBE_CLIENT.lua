--------------------------------------------------------------

-- L_NT_ASSEMBLYTUBE_CLIENT.lua

-- Handles processing of Assembly Teleporter animation loads
-- created abeechler ... 4/21/11 - refactored scripts and simplified functionality

--------------------------------------------------------------

local defaultTransportSendAnim = "tube-sucker"			-- Transport send animation
local transportReceiveAnim = "tube-resurrect"			-- Transport receive animation

----------------------------------------------
-- Adjust the send anim if overridden in config data
----------------------------------------------
function onStartup(self,msg)
	-- Set send anim if overridden
	local overrideAnim = self:GetVar("OverrideAnim")
	if(overrideAnim) then
	    defaultTransportSendAnim = overrideAnim
	end
end

----------------------------------------------
-- Ensure necessary anim loading for the suck tubes
----------------------------------------------
function preloadTubeAnims(self, player)
    -- Ensure the use player has all the necessary objects ready for use
	player:PreloadAnimation{animationID = defaultTransportSendAnim, respondObjID = self}
	player:PreloadAnimation{animationID = transportReceiveAnim, respondObjID = self}

end

----------------------------------------------
-- Catch when the local player comes within ghosting distance
----------------------------------------------
function onScopeChanged(self, msg)
    -- Has the player entered ghosting range?
    if(msg.bEnteredScope) then
        -- Obtain the local player object
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        if((not player:Exists()) or (not player:GetPlayerReady().bIsReady)) then
            -- Subscribe to a zone control object notification alerting the script
            -- when the local player object is ready
            self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID(), messageName="PlayerReady"}
            return
        end
        
        -- Custom function to preload necessary venture cannon animations
        preloadTubeAnims(self, player)
    end
end

----------------------------------------------
-- The zone control object says when the player is loaded
----------------------------------------------
function notifyPlayerReady(self, zoneObj, msg)
    -- Get the player
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    -- Custom function to preload necessary venture cannon animations
    preloadTubeAnims(self, player)
    -- Cancel the notification request
    self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end
