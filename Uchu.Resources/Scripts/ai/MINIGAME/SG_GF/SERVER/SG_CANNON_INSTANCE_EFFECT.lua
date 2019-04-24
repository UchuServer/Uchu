--------------------------------------------------------------
-- Description:
--
-- Server script for Shooting Gallery Actors in the instance.
-- This object will register with the zone script who will 
-- then trigger animations and actions on this object.
--
--------------------------------------------------------------


--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self)

	-- send an object loaded message to the ZoneControl object
	GAMEOBJ:GetZoneControlID():ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}
	
end


