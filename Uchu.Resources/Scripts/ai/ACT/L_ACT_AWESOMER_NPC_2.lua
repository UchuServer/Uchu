--///////////////////////////////////////////////////////////////////////////////////////
--//            Team Awesomer NPC -- SERVER Script
--///////////////////////////////////////////////////////////////////////////////////////

CONSTANTS = {}

-- @TODO:ISSUE - Lua cannot send client/single messages to non client OBJID's. Need a 
--               way to RerouteMessage in lua

function onMissionDialogueOK(self, msg)

	-- get the user
	local user = msg.responder


	-- tell zone control to prep the player to race
	GAMEOBJ:GetZoneControlID():MessageBoxRespond{iButton = 1, identifier = "Race_Cannon_Mission", sender = user}
	

end