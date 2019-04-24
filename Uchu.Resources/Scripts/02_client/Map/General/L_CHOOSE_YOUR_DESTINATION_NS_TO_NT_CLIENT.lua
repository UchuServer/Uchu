--------------------------------------------------------------
-- Displays a fancy animation as you walk up to the LUP door pad
--
-- updated mrb... 4/15/11 - added zone summary and new functionality
--------------------------------------------------------------

----------------------------------------------
-- Adjust the interact display icon on set-up
----------------------------------------------
function CheckInit(self)
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then 
		-- tell the zone control object to tell the script when the local player is loaded
		self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
	else	
		-- custom function
		Init(self, player)
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	
	-- custom function to set up the player options
	Init(self, player)

	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

function Init(self, player)
	local choiceZoneID = self:GetVar("choiceZone")
	
	-- if the player is on LUP then set up for choice box
    if LEVEL:GetCurrentZoneID() == choiceZoneID then
		local visitedZones = player:GetLocationsVisited().locations or {}
		
		-- check what map the player is in to see if we need the choicebox 
		for k, zoneID in ipairs(visitedZones) do
			if zoneID == 1900 then
				-- Set AM Console Variables
				self:SetVar("teleportTooltip", "")
				self:SetVar("bChoice", true)
				
				break
			end
		end
		
	end	
	
	self:SetProximityRadius{iconID = objAlertIconID, radius = 80, name = "Icon_Display_Distance"}
end

----------------------------------------------
-- Checking for distance based termination of interaction
-- to ensure proper shtudown of open interaction windows
----------------------------------------------
function ClearChoice(self) 
	-- if this is a choicebox close it
	if self:GetVar("bChoice") then	
	    UI:SendMessage("ToggleChoiceBox", {{"visible", false}})
	end
end 