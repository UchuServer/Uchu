--------------------------------------------------------------

-- LEGO Club door Client Script

--

-- updated mrb... 4/15/11 - added zone summary and new functionality

--------------------------------------------------------------

require('L_BASE_CONSOLE_TELEPORT_CLIENT')

require('L_CHOOSE_YOUR_DESTINATION_NS_TO_NT_CLIENT')



-- settings to goto LUP Station

local teleportTooltip = "UI_TOOLTIP_INTERACT_TO_TRAVEL_TO_LEGO_CLUB"

local objAlertIconID = 87		-- Object Proximity Interact Icon ID 

local choiceZoneID = 1700



----------------------------------------------

-- Adjust the interact display icon on set-up

----------------------------------------------

function onStartup(self)

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

		

    -- check to see if the help window is open

    if self:GetVar("bInUse") then     		

		-- close the UI window because the interaction was terminated should return iButton -1

		UI:SendMessage("ToggleInstanceEnter", {{"visible", false}})

		UI:SendMessage( "popGameState", {{"state", "Lobby"}} )

    end

    

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

