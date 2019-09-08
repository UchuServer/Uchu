
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('L_ZONE_AG_BURNO')
require('L_ZONE_AG_STAGE_PLATFORMS')
require('L_ZONE_AG_INSTRUMENTS')
require('L_ZONE_AG_STAGE_CHOICEBUILDS')

--------------------------------------------------------------
-- startup
--------------------------------------------------------------
function onStartup( self )

	onStartupBurno( self )

end


function onNotifyObject(self, msg)
	onNotifyObjectStageChoicebuilds(self, msg)
end

--------------------------------------------------------------
-- When objects are loaded via zone notification
--------------------------------------------------------------
function onObjectLoaded( self, msg )

	onObjectLoadedBurno( self, msg )
	onObjectLoadedStagePlatforms( self, msg )
	onObjectLoadedInstruments( self, msg )
		
end




--------------------------------------------------------------
-- player left zone
--------------------------------------------------------------
function onPlayerExit( self, msg )
	
	onPlayerExitInstruments( self, msg )
	
end


