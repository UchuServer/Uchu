require('o_mis')
require('client/ai/SND/L_AUDIO_CLIENT')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	
    SetProximityDistance(self, 1)
	
	AddInteraction(self, "interactionPlayerAnim", "shocked")
    
end

