--------------------------------------------------------------

-- L_VIS_TOGGLE_OBJ.lua

-- Catches and processes notification events for visibiltiy 
-- toggle objects as sent from a TOGGLE_NOTIFIER object
-- created abeechler - 6/8/11
--------------------------------------------------------------

----------------------------------------------------------------
-- Define empty tables that will be set from the 
-- specific notification object
----------------------------------------------------------------
local VisibilityObjectTable = {}

----------------------------------------------------------------
-- Variables passed from the object specific script that are used throughout this utility script
----------------------------------------------------------------
function setGameVariables(self, passedVisibilityObjectTable)

	VisibilityObjectTable = passedVisibilityObjectTable
	
end


---------------------------------------------------------------
-- The player has entered (or left) the ghosting range of the object
---------------------------------------------------------------
function baseOnScopeChanged(self, msg)
	-- If the player entered ghosting range
    if msg.bEnteredScope then  
		local player = GAMEOBJ:GetControlledID()
		if not player:Exists() then
			-- Tell the zone control object to notify the script when the local player is loaded
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
			return
		end
		-- Utility function used to process object visibility
		-- based on mission state
		CheckMissions(self, player)
	end
end

function onScopeChanged(self, msg)
	baseOnScopeChanged(self, msg)
end

---------------------------------------------------------------
-- We have caught a player loaded notification
---------------------------------------------------------------
function baseNotifyPlayerReady(self, zoneObj, msg)
	-- Get the player
	local player = GAMEOBJ:GetControlledID()
	
	-- Utility function used to process object visibility
    -- based on mission state
	CheckMissions(self, player)
	
	-- Cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

function notifyPlayerReady(self, zoneObj, msg)
	baseNotifyPlayerReady(self, zoneObj, msg)
end

---------------------------------------------------------------
-- Utility function used to process a player's current state on missions
-- responsible for updating this object's visibility
---------------------------------------------------------------
function CheckMissions(self, player)
	-- Get the name of the spawn network this object is on
	local mySpawnerNom = self:GetVar("spawner_name")
	
	if(VisibilityObjectTable[mySpawnerNom]) then
	    -- We have a valid associated mission list to process
	    for i, missionID in ipairs(VisibilityObjectTable[mySpawnerNom]) do
	        local missionstate = player:GetMissionState{missionID = missionID}.missionState
	        -- Determine if the player is on the mission
		    if missionstate == 2 or missionstate == 10 then
			    -- The player is on the mission, turn the object visiblity on
			    self:SetVisible{visible = true, fadeTime = 1}
			    return
		    end
	    end
	    
	    -- Turn the object visiblity off
	    self:SetVisible{visible = false}
	end
	
end

---------------------------------------------------------------
-- Additional visibility toggle utility provided for server 
-- objects monitoring current mission completion status
---------------------------------------------------------------
function baseOnNotifyClientObject(self,msg)
	if msg.name == "SetVisibility" then
		if((msg.param1 == 1) and (not self:GetVisible().visible)) then
			self:SetVisible{visible = true, fadeTime = 1}
		elseif msg.param1 == 0 then
			self:SetVisible{visible = false, fadeTime = 1}
		end
	end
end

function onNotifyClientObject(self,msg)
	baseOnNotifyClientObject(self,msg)
end
