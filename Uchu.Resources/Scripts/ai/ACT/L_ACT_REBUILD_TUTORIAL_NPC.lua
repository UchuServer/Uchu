--///////////////////////////////////////////////////////////////////////////////////////
--//            Rebuild Tutorial NPC -- SERVER Script
--///////////////////////////////////////////////////////////////////////////////////////

CONSTANTS = {}
CONSTANTS["CLIENT_TOOLTIP_MISSION_ACCEPT"] = 0
CONSTANTS["CLIENT_TOOLTIP_MISSION_COMPLETE"] = 1

-- @TODO:ISSUE - Lua cannot send client/single messages to non client OBJID's. Need a 
--               way to RerouteMessage in lua

function onMissionDialogueOK(self, msg)

	-- get the user
	local user = msg.responder

	-- on accept
	if (msg.bIsComplete == false) then
	
		-- tell the zone controller we are loaded (this will show the activator)
		GAMEOBJ:GetZoneControlID():ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}
	
		-- pop a tooltip on the player
		--self:Help{rerouteID = user, iHelpID = CONSTANTS["CLIENT_TOOLTIP_MISSION_ACCEPT"]}

	-- on complete	
	else

		-- pop a tooltip on the player
		--self:Help{rerouteID = user, iHelpID = CONSTANTS["CLIENT_TOOLTIP_MISSION_COMPLETE"]}
	
	end

end