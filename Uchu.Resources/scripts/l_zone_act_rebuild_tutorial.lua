--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
-- Start Location for the Zone
CONSTANTS = {}
CONSTANTS["PLAYER_START_POS"] = {x = -15.711255, y = 276.552267, z = -13.312634}
CONSTANTS["PLAYER_START_ROT"] = {w = 0.91913521289825, x = 0, y = 0.39394217729568, z = 0}
CONSTANTS["NPC_START_POS"] = { x = 156.0, y = 270.06, z = 145.0 }

-- Other constants
CONSTANTS["PLAYER_FACTION"] = 1
CONSTANTS["REBUILD_1_TEMPLATEID"] = 2451
CONSTANTS["REBUILD_NPC_TEMPLATEID"] = 2502


--------------------------------------------------------------
-- Helper Functions
--------------------------------------------------------------

--------------------------------------------------------------
-- store an object by name
--------------------------------------------------------------
function storeObjectByName(self, varName, object)

    idString = object:GetID()
    finalID = "|" .. idString
    self:SetVar(varName, finalID)
   
end


--------------------------------------------------------------
-- get an object by name
--------------------------------------------------------------
function getObjectByName(self, varName)

    targetID = self:GetVar(varName)
    if (targetID) then
		return GAMEOBJ:GetObjectByID(targetID)
	else
		return nil
	end
	
end


--------------------------------------------------------------
-- try to show rebuild activator
--------------------------------------------------------------
function showRebuildActivator(self)

	-- get the cannon
	local rebuild = getObjectByName(self, "rebuildObject")
	
	-- get the player
	local npc = getObjectByName(self, "rebuildNPCObject")
	
	-- if we have both show it
	if ((rebuild) and (npc)) then

		-- show the rebuild activator
		rebuild:DisplayRebuildActivator{bShow = true}

	end
	
		
end


--------------------------------------------------------------
-- Game Message Handlers
--------------------------------------------------------------

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self) 



end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    
end    


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)
	
end


--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)

end


--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
function onObjectLoaded(self, msg)

	-- Rebuild Object Loaded
	if (msg.templateID == CONSTANTS["REBUILD_1_TEMPLATEID"]) then

		-- store the rebuild object for use later
		storeObjectByName(self, "rebuildObject", msg.objectID)

	elseif (msg.templateID == CONSTANTS["REBUILD_NPC_TEMPLATEID"]) then
	
		-- store the rebuild npc object for use later
		storeObjectByName(self, "rebuildNPCObject", msg.objectID)
		
		showRebuildActivator(self)

	end

end

