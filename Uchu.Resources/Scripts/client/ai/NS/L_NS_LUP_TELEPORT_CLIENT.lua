--------------------------------------------------------------
-- Displays a fancy animation as you walk up to the LUP door pad
--
-- updated mrb... 4/15/11 - added zone summary and new functionality
--------------------------------------------------------------
require('02_client/Map/General/L_BASE_CONSOLE_TELEPORT_CLIENT')
require('02_client/Map/General/L_CHOOSE_YOUR_DESTINATION_NS_TO_NT_CLIENT')

-- settings to goto LUP Station
local teleportTooltip = "ROCKET_TOOLTIP_USE_THE_GATEWAY_TO_TRAVEL_TO_LUP_WORLD"
local objAlertIconID = 94		-- Object Proximity Interact Icon ID 
local choiceZoneID = 1600

----------------------------------------------
-- Adjust the interact display icon on set-up
----------------------------------------------
function onStartup(self)
	if LEVEL:GetCurrentZoneID() == choiceZoneID then
		objAlertIconID = 95
	end
	
	self:SetVar("choiceZone", choiceZoneID)
	self:SetVar("objAlertIconID", objAlertIconID)
	self:SetVar("teleportTooltip", teleportTooltip)
	-- run startup
	baseStartup(self)
end

function onScopeChanged(self, msg)
	if not msg.bEnteredScope then return end
	
	-- see if the player is ready to init
	CheckInit(self)
end

----------------------------------------------
-- Check to see if the player can use the console
----------------------------------------------
function onCheckUseRequirements(self, msg)
	return baseCheckUseRequirements(self, msg)
end

----------------------------------------------
-- Code script info request for the custom npc icon display
----------------------------------------------
function onGetInteractionDetails(self, msg) 
	return baseGetInteractionDetails(self, msg)
end

----------------------------------------------
-- Checking for distance based termination of interaction
-- to ensure proper shtudown of open interaction windows
----------------------------------------------
function onTerminateInteraction(self, msg) 
	-- if this is a choicebox close it
	ClearChoice(self)
	-- run terminateInteraction
	baseTerminateInteraction(self, msg)
end

--------------------------------------------------------------
-- Sent when a player enter/leave a Proximity Radius
--------------------------------------------------------------
function onProximityUpdate(self, msg)
	baseProximityUpdate(self, msg)
end

function onGetPriorityPickListType(self, msg)	
    return baseGetPriorityPickListType(self, msg)
end 