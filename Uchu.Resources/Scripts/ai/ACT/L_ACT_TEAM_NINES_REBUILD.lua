require('State')
require('o_onEvent')
require('o_mis')

--///////////////////////////////////////////////////////////////////////////////////////
--//            Team Nine Times Platform Rebuild -- SERVER Script
--///////////////////////////////////////////////////////////////////////////////////////

function onStartup(self) 

	-- break the rebuild
	-- For some reason, this doesn't work from the zone script, though the rebuild cannon does
	self:RebuildReset()
     
end
     
