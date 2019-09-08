require('o_mis')
require('L_AUDIO_CLIENT')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	
	SetProximityDistance(self, 10)
	
	
	
    AddInteraction(self, "proximityEffect", "proxsnd")

    
end

