require('State')
require('o_onEvent')
require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            Rebuild Tutorial -- SERVER Script
--///////////////////////////////////////////////////////////////////////////////////////

function onStartup(self) 

	-- tell the zone controller we are loaded
	GAMEOBJ:GetZoneControlID():ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}

	-- Hide the activator until someone requests it
	self:DisplayRebuildActivator{bShow = false}
	
	-- break the rebuild
	self:RebuildReset()
     
end
     
